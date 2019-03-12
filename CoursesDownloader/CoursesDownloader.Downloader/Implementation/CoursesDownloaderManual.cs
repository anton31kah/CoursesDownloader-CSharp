using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.AdvancedIO.SpecialActions;
using CoursesDownloader.Client;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.Downloader.Implementation.Helpers;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsShortcut;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Downloader.Implementation
{
    public static class CoursesDownloaderManual
    {
        public static async Task Init()
        {
            ConsoleUtils.OutputEncoding = Encoding.UTF8;

            await CoursesClient.LazyRefresh();

            SharedVars.CurrentSemesterNumber = await CoursesExtractor.ExtractSemestersCount();
        }

        private static async Task<ICourseLink> AskForCourse()
        {
            await CoursesExtractor.ExtractCourses();
            var result = MenuChooseItem.AskInputForSingleItemFromList(SharedVars.Courses, "course");
            SharedVars.SelectedCourseLink = result;
            return result;
        }

        private static async Task<ISection> AskForSection()
        {
            await SectionExtractor.ExtractSectionsForCourse(SharedVars.SelectedCourseLink);
            var result = MenuChooseItem.AskInputForSingleItemFromList(SharedVars.Sections, "section");
            SharedVars.SelectedSection = result;
            return result;
        }

        private static IEnumerable<IDownloadableLink> AskForMultipleLinks()
        {
            var result = MenuChooseItem.AskInputForMultipleItemsFromList(SharedVars.SelectedSection.Links, "files").ToList();
            SharedVars.DownloadQueue.AddUnique(result);
            return result;
        }

        private static async Task<NamingMethod> AskForNamingMethod()
        {
            const string secondChoice = "Use the file name that appears on courses (Recommended)";
            const string examplesChoice = "Show examples and ask again (will show random 5 examples, BUT slower)";

            var choicesPossible = new List<string>
            {
                "Use the file name from the url (default behavior when downloading from browser)",
                secondChoice,
                examplesChoice
            };

            while (true)
            {
                var result = MenuChooseItem.AskInputForSingleItemFromList(choicesPossible, "choice", "choose");

                if (result == choicesPossible[0])
                {
                    SharedVars.NamingMethod = NamingMethod.UrlName;
                    break;
                }

                if (result == choicesPossible[1])
                {
                    SharedVars.NamingMethod = NamingMethod.CoursesName;
                    break;
                }

                if (result == choicesPossible[2])
                {
                    // because we're displaying in place but AskInputForSingleItemFromList still appends to breadcrumbs
                    SharedVars.ChosenItemsTillNow.Remove(SharedVars.ChosenItemsTillNow.Keys.Last());

                    choicesPossible.Remove(examplesChoice);
                    var randomN = SharedVars.DownloadQueue.TakeRandomN(5).ToList();

                    await HandleExternalLinksThatAreFiles(randomN);
                    
                    var table = new ConsoleTableUtil("From Courses", "From URL (Default)");

                    var randomNLinksTasks = new List<Task>();

                    foreach (var link in randomN)
                    {
                        randomNLinksTasks.Add(
                            link.GetNameFromUrlNow()
                                .ContinueWith(task =>
                                    table.AddRow(
                                        link.FileFromCourses.FileNameAndExtensionOnly,
                                        link.FileFromUrl.FileNameAndExtensionOnly
                                    )
                                )
                        );
                    }

                    await Task.WhenAll(randomNLinksTasks);

                    var examples = $"Some examples:\n\n{table}";

                    choicesPossible.Remove(secondChoice);
                    choicesPossible.Add($"{secondChoice}\n{examples}");
                }
            }


            return SharedVars.NamingMethod;
        }

        public static async Task Run()
        {
            SharedVars.CurrentRunningActionType = RunningActionType.AskForCourse;
            while (true)
            {
                try
                {
                    switch (SharedVars.CurrentRunningActionType)
                    {
                        case RunningActionType.AskForCourse:
                            await AskForCourse();
                            break;
                        case RunningActionType.AskForSection:
                            await AskForSection();
                            break;
                        case RunningActionType.AskForMultipleLinks:
                            AskForMultipleLinks();
                            break;
                        case RunningActionType.AskForNamingMethod:
                            await AskForNamingMethod();
                            break;
                        case RunningActionType.DownloadSelectedLinks:
                            await DownloadSelectedLinks();
                            break;
                    }

                    SharedVars.CurrentRunningActionType++;
                }
                catch (BaseAction action)
                {
                    await ActionHandler.HandleAction(action);
                }

                if (SharedVars.CurrentRunningActionType == RunningActionType.Repeat)
                {
                    var startAgain = MenuChooseItem.AskYesNoQuestion("Do you want to start again? [Y/N] ",
                        () =>
                        {
                            SharedVars.ChosenItemsTillNow.Clear();
                            SharedVars.CurrentRunningActionType = RunningActionType.AskForCourse;
                        }, 
                        Dispose);

                    if (!startAgain) // onNo
                    {
                        break;
                    }
                }

                if (SharedVars.CurrentRunningActionType == RunningActionType.End)
                {
                    Dispose();
                    break;
                }
            }
        }

        private static async Task DownloadSelectedLinks()
        {
            await HandleExternalLinksThatAreFiles();

            var totalLen = SharedVars.DownloadQueue.Count;

            ConsoleUtils.Clear();

            ProgressBarUtil.InitMainProgressBar(totalLen);
            
            var extraPathPerLink = ExtraPathsHelper.FillExtraPaths();

            var downloadBatch = new List<Task>();
            
            const int batch = 5;
            var started = 0;

            // solution provided from here https://stackoverflow.com/a/54894420/6877477
            foreach (var link in SharedVars.DownloadQueue)
            {
                extraPathPerLink.TryGetValue(link, out var middlePath);

                var task = TriggerDownloadFile(link, middlePath, ++started, totalLen);
                downloadBatch.Add(task);

                if (downloadBatch.Count == batch)
                {
                    // Wait for any Task to complete, then remove it from the list of pending tasks.
                    var completedTask = await Task.WhenAny(downloadBatch);
                    downloadBatch.Remove(completedTask);
                }
            }

            // Wait for all of the remaining Tasks to complete
            await Task.WhenAll(downloadBatch);

            
            ProgressBarUtil.TickMain($"Downloaded all {totalLen} / {totalLen}");

            ProgressBarUtil.Dispose();

            ConsoleUtils.Clear();

            ConsoleUtils.WriteLine(SharedVars.DownloadQueue
                                            .Select((link, idx) => $"{idx + 1}. {link.Name}")
                                            .Prepend("Downloaded:")
                                            .Join()
            );

            SharedVars.DownloadQueue.Clear();


            ConsoleUtils.WriteLine();
        }

        private static async Task HandleExternalLinksThatAreFiles(IList<IDownloadableLink> required = null)
        {
            var externalLinks = SharedVars.DownloadQueue.OfType<IExternalLink>().ToList();

            foreach (var externalLink in externalLinks)
            {
                var indexInRequired = required?.IndexOf(externalLink) ?? -1;
                if (required != null && indexInRequired < 0)
                {
                    continue;
                }

                try
                {
                    await externalLink.GetNameFromUrlNow();
                }
                catch (ExternalUrlIsFileException e)
                {
                    var indexInDownloadQueue = SharedVars.DownloadQueue.FindIndex(d => e.ExternalLink.Equals(d));
                    SharedVars.DownloadQueue[indexInDownloadQueue] = e.FileLink;

                    if (required != null && indexInRequired >= 0)
                    {
                        await e.FileLink.GetNameFromUrlNow();
                        required[indexInRequired] = e.FileLink;
                    }
                }
            }
        }

        private static async Task TriggerDownloadFile(IDownloadableLink link, string[] middlePath, int started, int totalLen)
        {
            ProgressBarUtil.InitFileProgressBar(link);
            ProgressBarUtil.TickMain($"Downloading... {started} / {totalLen}");
            await link.Download(middlePath);
        }
        
        public static void Dispose()
        {
            CoursesClient.Dispose();
        }
    }
}