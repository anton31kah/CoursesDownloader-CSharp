namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers.ProgressBarUtilHelpers
{
    public class MovingAverage
    {
        private double _totalSum;
        private int _totalCount;
        public double Average { get; private set; }

        public double ComputeAverage(double newValue)
        {
            _totalSum += newValue;
            _totalCount++;

            Average = _totalSum / _totalCount;

            return Average;
        }
    }
}