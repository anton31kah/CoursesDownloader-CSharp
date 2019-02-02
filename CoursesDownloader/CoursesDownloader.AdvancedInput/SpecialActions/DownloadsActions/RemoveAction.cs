namespace CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions
{
    public class RemoveAction : QueueModificationBaseAction
    {
        protected override string Type => "Remove";

        protected override string ToFromWord => "From";
    }
}