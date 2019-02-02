using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions;
using CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions.QueueModificationBaseActionHelpers;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Downloader.Implementation.ExtractorsAndHandlers
{
    internal static class ActionHandler
    {
        internal static async Task HandleRefreshAction()
        {
            await CoursesExtractor.ExtractCourses();
            if (SharedVars.SelectedCourseLink != null)
            {
                await SectionExtractor.ExtractSectionsForCourse(SharedVars.SelectedCourseLink);
            }
        }

        internal static void HandleAddRemoveActions(QueueModificationBaseAction action)
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
    }
}