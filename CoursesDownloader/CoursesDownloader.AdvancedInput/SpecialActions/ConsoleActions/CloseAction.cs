using System;

namespace CoursesDownloader.AdvancedIO.SpecialActions.ConsoleActions
{
    public class CloseAction : BaseAction
    {
        protected override string Type => "Close";
        protected override string Description => "Closes the program";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;
            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction($"{Type.ToLower()} the program",
                    () =>
                    {
                        State = ActionState.FoundAndHandled;
                    });
            }

            return this;
        }
    }
}