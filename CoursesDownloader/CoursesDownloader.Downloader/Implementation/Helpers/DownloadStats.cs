namespace CoursesDownloader.Downloader.Implementation.Helpers
{
    public class DownloadStats
    {
        public int Done { get; private set; }
        public int InProgress { get; private set; }
        public int Waiting { get; private set; }
        public int Total { get; private set; }

        public DownloadStats(int total)
        {
            Done = default;
            InProgress = default;
            Waiting = total;
            Total = total;
        }

        public void Start()
        {
            ++InProgress;
            --Waiting;
        }

        public void End()
        {
            --InProgress;
            ++Done;
        }

        public override string ToString()
        {
            return $"{Done} Done | {InProgress} In Progress | {Waiting} Left | {Total} Total";
        }
    }
}