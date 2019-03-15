using System.Linq;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions
{
    public class DownloadAction : BaseAction
    {
        protected override string Type => "Download";
        protected override string Description => "Starts downloading the download queue";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("start downloading the download queue",
                    () =>
                    {
                        if (SharedVars.DownloadQueue.Any())
                        {
                            State = ActionState.FoundAndHandled;
                        }
                        else
                        {
                            ConsoleUtils.WriteLine("Download queue is empty", ConsoleIOType.Error);
                            State = ActionState.NotFound;
                        }
                    });
            }

            return this;
        }

        public override void SetNextRunningActionType()
        {
            SharedVars.CurrentRunningActionType = RunningActionType.AskForNamingMethod;
        }
    }
}