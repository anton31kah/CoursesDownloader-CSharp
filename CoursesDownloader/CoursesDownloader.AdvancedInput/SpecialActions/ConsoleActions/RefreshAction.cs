namespace CoursesDownloader.AdvancedIO.SpecialActions.ConsoleActions
{
    public class RefreshAction : BaseAction
    {
        protected override string Type => "Refresh";
        protected override string Description => "Refreshes everything you selected up to now";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
            {
                ConfirmAction("refresh everything you selected",
                    () =>
                    {
                        State = ActionState.FoundAndHandled;
                    });
            }

            return this;
        }
    }
}