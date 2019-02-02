using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.AdvancedIO.SpecialActions;
using CoursesDownloader.AdvancedIO.SpecialActions.ConsoleActions;
using CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions;
using CoursesDownloader.Client;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.Downloader.Implementation.ExtractorsAndHandlers;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Downloader.Implementation
{
    public static class CoursesDownloaderManual
    {
        public static async Task Init()
        {
            Console.OutputEncoding = Encoding.UTF8;

            await CoursesClient.LazyRefresh();
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
            SharedVars.DownloadQueue.Clear();
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
                    var randomN = SharedVars.DownloadQueue.TakeRandomN(5);
                    
                    var table = new ConsoleTableUtil("From Courses", "From URL (Default)");

                    foreach (var link in randomN)
                    {
                        await link.GetNameFromUrlNow();

                        table.AddRow(
                            link.FileFromCourses.FileNameAndExtensionOnly, 
                            link.FileFromUrl.FileNameAndExtensionOnly
                            );
                    }

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
                catch (QueueModificationBaseAction action)
                {
                    ActionHandler.HandleAddRemoveActions(action);
                }
                catch (RefreshAction)
                {
                    await ActionHandler.HandleRefreshAction();
                }
                catch (BaseAction action)
                {
                    action.SetNextRunningActionType();
                }

                if (SharedVars.CurrentRunningActionType == RunningActionType.Repeat)
                {
                    var startAgain = MenuChooseItem.AskYesNoQuestion("Do you want to start again? [Y/N] ",
                        () =>
                        {
                            SharedVars.ChosenItemsTillNow.Clear();
                            SharedVars.CurrentRunningActionType = RunningActionType.AskForCourse;
                        }, 
                        CoursesClient.Dispose);

                    if (!startAgain) // onNo
                    {
                        break;
                    }
                }
            }
        }

        private static async Task DownloadSelectedLinks()
        {
            var totalLen = SharedVars.DownloadQueue.Count;

            Console.Clear();

            ProgressBarUtil.InitMainProgressBar(totalLen);

            var i = 1;

            foreach (var link in SharedVars.DownloadQueue)
            {
                ProgressBarUtil.InitFileProgressBar(link);
                ProgressBarUtil.TickMain($"Downloading... {i} / {totalLen}");
                await link.Download();
                i++;
            }

            ProgressBarUtil.TickMain($"Downloaded all {i} / {totalLen}");

            ProgressBarUtil.Dispose();

            Console.Clear();

            Console.WriteLine(SharedVars.DownloadQueue
                                            .Select((link, idx) => $"{idx + 1}. {link.Name}")
                                            .Prepend("Downloaded:")
                                            .Join()
            );
                

            Console.WriteLine();
        }
    }
}