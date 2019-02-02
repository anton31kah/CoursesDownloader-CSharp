namespace CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions
{
    public class AddAction : QueueModificationBaseAction
    {
        protected override string Type => "Add";

        protected override string ToFromWord => "To";
    }
}