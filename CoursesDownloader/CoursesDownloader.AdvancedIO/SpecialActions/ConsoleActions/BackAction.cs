using System.Linq;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.ConsoleActions
{
    public class BackAction : BaseAction
    {
        protected override string Type => "Back";
        protected override string Description => "Goes back (up) the breadcrumbs one step";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("return you one step back",
                    () =>
                    {
                        State = ActionState.FoundAndHandled;
                        var pastStates = SharedVars.ChosenItemsTillNow.Keys.Any(r => r == SharedVars.CurrentRunningActionType - 1);
                        if (pastStates)
                        {
                            SharedVars.ChosenItemsTillNow.Remove(SharedVars.CurrentRunningActionType - 1);
                        }
                    });
            }

            return this;
        }

        public override void SetNextRunningActionType()
        {
            var previousRunningActionType = SharedVars.PreviousRunningActionTypes.Reverse().Skip(1).Take(1).ToArray();
            
            if (previousRunningActionType.Any())
            {
                SharedVars.CurrentRunningActionType = previousRunningActionType.First();
            }
        }
    }
}