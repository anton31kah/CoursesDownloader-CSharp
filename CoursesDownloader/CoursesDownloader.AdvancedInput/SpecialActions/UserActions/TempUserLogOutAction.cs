using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.UserActions
{
    public class TempUserLogOutAction : BaseAction
    {
        protected override string Type => "TempUserLogOut";

        protected override string Description => "Log out the temp user, and log in as the default user " +
                                                 "(log in as the temp again with TempUserLogIn)";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("log out of the temp user",
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