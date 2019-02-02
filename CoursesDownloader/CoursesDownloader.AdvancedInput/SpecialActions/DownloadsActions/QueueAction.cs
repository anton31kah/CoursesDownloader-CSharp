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

                var totalPages = (int) Math.Ceiling(CommonVars.DownloadQueue.Count / 20.0);

                linksPage = CommonVars.DownloadQueue.Skip(page * pageSize).Take(pageSize)
                    .Select((link, i) => $"{i + 1}. {link.Name}").Join();

                ConsoleUtils.WriteLine(linksPage, ConsoleColor.White);

                var pagesItemsTotal =
                    $"page {page + 1}/{totalPages} | items {CommonVars.DownloadQueue.Count}";

                message = CommonVars.DownloadQueue.Skip((page + 1) * pageSize).Take(pageSize).Any()
                    ? $"Want to see more ({pagesItemsTotal})? [Y/N] "
                    : "Again (Empty Queue)? [Y/N] ";

                page++;
                if (page * pageSize > CommonVars.DownloadQueue.Count)
                {
                    page = 0;
                }

            } while (MenuChooseItem.AskYesNoQuestion(message,
                onOther: fullInputString =>
                {
                    page = OnInQueueRemoveAction(fullInputString, linksPage, page, pageSize);
                }));
        }

        private static int OnInQueueRemoveAction(string fullInputString, string linksPage, int page, int pageSize)
        {
            if (fullInputString.ToLower().Contains("clear"))
            {
                CommonVars.DownloadQueue.Clear();
                return 0;
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
                        CommonVars.DownloadQueue.Clear();
                        return 0;
                    }
                }

                if (sequence != null)
                {
                    page--;

                    var matchingLinks = CommonVars.DownloadQueue.Skip(page * pageSize)
                        .Take(pageSize)
                        .Where((_, i) => sequence.Contains(i));
                    CommonVars.DownloadQueue.RemoveAll(link => matchingLinks.Contains(link));
                }
            }

            return page;
        }
    }
}