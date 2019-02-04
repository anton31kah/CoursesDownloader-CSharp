using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.UserActions
{
    public class SwitchSemesterAction : BaseAction
    {
        protected override string Type => "SwitchSemester";

        protected override string Description => "Switch to another semester";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("switch to another semester",
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