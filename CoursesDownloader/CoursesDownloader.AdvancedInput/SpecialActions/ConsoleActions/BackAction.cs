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
                        var pastStates = CommonVars.ChosenItemsTillNow.Keys;
                        if (pastStates.Any())
                        {
                            CommonVars.ChosenItemsTillNow.Remove(pastStates.Last());
                        }
                    });
            }

            return this;
        }

        public override int SetActionIdxPointer(int level)
        {
            return level > 0 ? level - 1 : 0;
        }
    }
}