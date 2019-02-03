using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.AdvancedIO.SpecialActions;
using CoursesDownloader.AdvancedIO.SpecialActions.ConsoleActions;
using CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions;
using CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions.QueueModificationBaseActionHelpers;
using CoursesDownloader.AdvancedIO.SpecialActions.UserActions;
using CoursesDownloader.Client;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Downloader.Implementation.ExtractorsAndHandlers
{
    internal static class ActionHandler
    {
        internal static async Task HandleAction(BaseAction action)
        {
            switch (action)
            {
                case CloseAction _:
                    HandleCloseAction();
                    break;

                case LogOutAction _:
                    await HandleLogOutAction();
                    break;

                case QueueModificationBaseAction queueModificationBaseAction:
                    HandleQueueModificationBaseAction(queueModificationBaseAction);
                    break;

                case RefreshAction _:
                    await HandleRefreshAction();
                    break;

                case SwitchSemesterAction _:
                    await HandleSwitchSemesterAction();
                    break;

                case TempUserLogInAction _:
                    await HandleTempUserLogInAction();
                    break;

                case TempUserLogOutAction _:
                    HandleTempUserLogOutAction();
                    break;
            }

            action.SetNextRunningActionType();
        }

        private static void HandleCloseAction()
        {
            CoursesClient.Dispose();
        }

        private static async Task HandleLogOutAction()
        {
            CoursesClient.Dispose();
            await CoursesDownloaderManual.Init();
        }

        private static void HandleQueueModificationBaseAction(QueueModificationBaseAction action)
        {
            var matchingLinks = new List<IDownloadableLink>();

            switch (action.MatchingItemType)
            {
                case ItemTypeToAddRemove.Course:
                    matchingLinks = SharedVars.Courses
                        .Where((course, j) => action.MatchingItems.Contains(j))
                        .SelectMany(course => SectionExtractor.ExtractSectionsForCourse(course).Result
                            .SelectMany(section => section.Links))
                        .ToList();
                    break;
                case ItemTypeToAddRemove.Section:
                    matchingLinks = SharedVars.Sections
                        .Where((section, j) => action.MatchingItems.Contains(j))
                        .SelectMany(section => section.Links)
                        .ToList();
                    break;
                case ItemTypeToAddRemove.Link:
                    matchingLinks = SharedVars.SelectedSection.Links
                        .ToList();
                    break;
            }

            do
            {
                string message;
                if (action is AddAction)
                {
                    
                    var count = matchingLinks.Except(SharedVars.DownloadQueue).Count();
                    SharedVars.DownloadQueue.AddUnique(matchingLinks);

                    message = $"Added {count} items (to revert, " +
                              "simply call Remove like you did with Add in the same way and location";
                }
                else
                {
                    var count = SharedVars.DownloadQueue.Intersect(matchingLinks).Count();
                    SharedVars.DownloadQueue.RemoveAll(link => matchingLinks.Contains(link));

                    message = $"Removed {count} items (to revert, " +
                              "simply call Add like you did with Remove in the same way and location";
                }

                ConsoleUtils.WriteLine(message, ConsoleColor.Yellow);

                if (action is AddAction)
                {
                    action = new RemoveAction();
                }
                else
                {
                    action = new AddAction();
                }

            } while (MenuChooseItem.AskYesNoQuestion("Do you want to revert now? [Y/N] "));
        }

        private static async Task HandleRefreshAction()
        {
            await CoursesExtractor.ExtractCourses();
            if (SharedVars.SelectedCourseLink != null)
            {
                await SectionExtractor.ExtractSectionsForCourse(SharedVars.SelectedCourseLink);
            }
        }

        private static async Task HandleSwitchSemesterAction()
        {
            var namedSemesters = new List<string>
            {
                "1st Semester",
                "2nd Semester",
                "3rd Semester"
            };
            namedSemesters.AddRange(Enumerable.Range(4, 5).Select(i => $"{i}th Semester"));

            var semestersCount = await CoursesExtractor.ExtractSemestersCount();
            
            var itemsList = namedSemesters.Take(semestersCount).ToList();

            var chosenItem = MenuChooseItem.AskInputForSingleItemFromList(itemsList, "semester", "switch to", breadcrumbs: false);

            var chosenSemester = namedSemesters.IndexOf(chosenItem) + 1;

            SharedVars.CurrentSemesterNumber = chosenSemester;
        }

        private static async Task HandleTempUserLogInAction()
        {
            await CoursesClient.TempLogInUser();
            await CoursesDownloaderManual.Init();
        }

        private static void HandleTempUserLogOutAction()
        {
            CoursesClient.TempLogOutUser();
        }
    }
}