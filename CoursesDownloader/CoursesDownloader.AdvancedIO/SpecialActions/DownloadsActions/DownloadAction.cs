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
                    () => { State = ActionState.FoundAndHandled; });
            }

            return this;
        }

        public override void SetNextRunningActionType()
        {
            SharedVars.CurrentRunningActionType = RunningActionType.AskForNamingMethod;
        }
    }
}