using System;
using System.Collections.Generic;
using System.Linq;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions
{
    public class QueueAction : BaseAction
    {
        protected override string Type => "Queue";
        protected override string Description => "Shows download queue";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("show the download queue", ShowQueue);
            }

            return this;
        }

        private void ShowQueue()
        {
            State = ActionState.FoundAndHandled;

            string linksPage;
            var page = 0;
            const int pageSize = 20;
            string message;

            do
            {
                Console.Clear();

                Console.WriteLine("Queue:");

                var totalPages = (int) Math.Ceiling(SharedVars.DownloadQueue.Count / 20.0);

                linksPage = SharedVars.DownloadQueue.Skip(page * pageSize).Take(pageSize)
                    .Select((link, i) => $"{i + 1}. {link.Name}").Join();

                ConsoleUtils.WriteLine(linksPage, ConsoleColor.White);

                message = FormatMessage(page, totalPages, pageSize, linksPage);

                page++;
                if (page * pageSize > SharedVars.DownloadQueue.Count)
                {
                    page = 0;
                }

            } while (MenuChooseItem.AskYesNoQuestion(message,
                onOther: fullInputString =>
                {
                    page = OnInQueueActionWords(fullInputString, linksPage, page, pageSize);
                }));
        }

        private static string FormatMessage(int page, int totalPages, int pageSize, string linksPage)
        {
            var currentPage = page + 1;
            var currentItemsFrom = page * pageSize + 1;
            var currentItemsTo = page * pageSize + linksPage.Count(c => c == '\n') + 1;
            var maxItems = SharedVars.DownloadQueue.Count;

            var pagesItemsViewString = $"page {currentPage}/{totalPages} | " +
                                       $"items {currentItemsFrom}-{currentItemsTo}/{maxItems}";
            pagesItemsViewString = SharedVars.DownloadQueue.Count == 0 ? "Empty Queue" : pagesItemsViewString;

            var message = SharedVars.DownloadQueue.Skip((page + 1) * pageSize).Take(pageSize).Any()
                ? $"Want to see more ({pagesItemsViewString})? [Y/N] "
                : $"Again ({pagesItemsViewString})? [Y/N] ";
            return message;
        }

        private static int OnInQueueActionWords(string fullInputString, string linksPage, int page, int pageSize)
        {
            if (fullInputString.ToLower().Contains("clear"))
            {
                SharedVars.DownloadQueue.Clear();
                return 0;
            }

            if (fullInputString.ToLower().Contains("download"))
            {
                throw new DownloadAction();
            }

            if (fullInputString.ToLower().Contains("remove"))
            {
                IEnumerable<int> sequence = null;
                foreach (var piece in fullInputString.Split(' '))
                {
                    sequence = MenuChooseItem.ValidateInput(piece);
                    if (sequence != null)
                    {
                        sequence = sequence.Select(i => i - 1)
                            .Where(i => i >= 0 &&
                                        i < linksPage.Split('\n').Length);
                        break;
                    }

                    if (piece.ToLower() == "all")
                    {
                        SharedVars.DownloadQueue.Clear();
                        return 0;
                    }
                }

                if (sequence != null)
                {
                    page = page > 0 ? --page : page;

                    var matchingLinks = SharedVars.DownloadQueue.Skip(page * pageSize)
                        .Take(pageSize)
                        .Where((_, i) => sequence.Contains(i));
                    SharedVars.DownloadQueue.RemoveAll(link => matchingLinks.Contains(link));
                }
            }

            return page;
        }
    }
}