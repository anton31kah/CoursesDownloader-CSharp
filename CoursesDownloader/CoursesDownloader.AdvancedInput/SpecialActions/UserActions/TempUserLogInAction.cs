using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.UserActions
{
    public class TempUserLogInAction : BaseAction
    {
        protected override string Type => "TempUserLogIn";

        protected override string Description => "Log out the current user, and log in as a temp user " +
                                                 "(ends by TempUserLogOut, or by closing the session (close, quit, exit))";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("log in as a temp user",
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