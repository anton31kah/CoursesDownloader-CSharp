using System;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers.ProgressBarUtilHelpers
{
    public struct TimeByteBpsStamp
    {
        public DateTime LastTimeStamp { get; private set; }
        public int LastBytesStamp { get; private set; }
        public MovingAverage AverageBps { get; }

        public TimeByteBpsStamp(DateTime lastTimeStamp)
        {
            LastTimeStamp = lastTimeStamp;
            LastBytesStamp = 0;
            AverageBps = new MovingAverage();
        }

        public TimeByteBpsStamp Update(DateTime lastTimeStamp, int lastBytesStamp)
        {
            LastTimeStamp = lastTimeStamp;
            LastBytesStamp = lastBytesStamp;
            return this;
        }
    }
}