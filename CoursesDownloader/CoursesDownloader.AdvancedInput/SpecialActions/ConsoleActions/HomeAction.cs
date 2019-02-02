using System.Linq;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.ConsoleActions
{
    public class HomeAction : BaseAction
    {
        protected override string Type => "Home";
        protected override string Description => "Goes up to the top of breadcrumbs (to the course selection menu)";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("take you back to course selection menu",
                    () =>
                    {
                        State = ActionState.FoundAndHandled;
                        var pastStates = SharedVars.ChosenItemsTillNow.Keys.ToArray();
                        foreach (var pastState in pastStates)
                        {
                            SharedVars.ChosenItemsTillNow.Remove(pastState);
                        }
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
