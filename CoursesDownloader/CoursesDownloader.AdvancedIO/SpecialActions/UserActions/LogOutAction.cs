using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.UserActions
{
    public class LogOutAction : BaseAction
    {
        protected override string Type => "LogOut";

        protected override string Description => "Log out and be prompted to enter login information again, " +
                                                 "this will disable auto login until the credentials are entered, " +
                                                 "and the stored ones will be completely disposed";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("log you out, you'll have to log in again",
                    () =>
                    {
                        State = ActionState.FoundAndHandled;
                    });
            }

            return this;
        }

        public override void SetNextRunningActionType()
        {
            SharedVars.CurrentRunningActionType = RunningActionType.AskForCourse;
        }
    }
}