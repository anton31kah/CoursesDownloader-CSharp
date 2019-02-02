using System;
using System.Collections.Generic;
using System.Net.Http.Handlers;
using ByteSizeLib;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers.ProgressBarUtilHelpers;
using ShellProgressBar;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers
{
    public static class ProgressBarUtil
    {
        private static readonly ProgressBarOptions ParentProgressBarOptions = new ProgressBarOptions
        {
            ProgressCharacter = '#',
            BackgroundCharacter = ' ',
            DisplayTimeInRealTime = false,
            EnableTaskBarProgress = true,
            ForegroundColor = ConsoleColor.White,
            ForegroundColorDone = ConsoleColor.White
        };

        private static readonly ProgressBarOptions ChildProgressBarOptions = new ProgressBarOptions
        {
            ProgressCharacter = '#',
            BackgroundCharacter = ' ',
            CollapseWhenFinished = false,
            ForegroundColor = ConsoleColor.Green,
            ForegroundColorDone = ConsoleColor.White
        };

        private static ProgressBar _parentProgressBar;

        private static readonly Dictionary<IDownloadableLink, ChildProgressBar> ChildrenProgressBars = new Dictionary<IDownloadableLink, ChildProgressBar>();
        private static TimeByteBpsStamp _timeByteBpsStamp;

        public static void InitMainProgressBar(int totalItems, string message = null)
        {
            message = message.IfIsNullOrEmpty($"Downloading... 0 / {totalItems}");
            _parentProgressBar = new ProgressBar(totalItems, message, ParentProgressBarOptions);
        }

        public static void InitFileProgressBar(IDownloadableLink link)
        {
            var childProgressBar = _parentProgressBar.Spawn((int) link.FileSize, $"Starting to download {link.Name}...", ChildProgressBarOptions);
            ChildrenProgressBars[link] = childProgressBar;
            _timeByteBpsStamp = new TimeByteBpsStamp(DateTime.MinValue);
        }

        public static void TickMain(string message)
        {
            _parentProgressBar.Tick(message);
        }

        public static void TickFile(DownloadableLink link, HttpProgressEventArgs progress)
        {
            var childProgressBar = ChildrenProgressBars[link];

            if (progress.TotalBytes != null)
            {
                childProgressBar.MaxTicks = (int) progress.TotalBytes;
            }

            // download size progress
            var transferred = (int) progress.BytesTransferred;
            var max = childProgressBar.MaxTicks;
            var bytesProgress = $"{FormatSize(transferred)} / {FormatSize(max)}";

            // eta
            var eta = CalculateEta(transferred, max);
            
            _timeByteBpsStamp.Update(DateTime.Now, transferred);

            var message = $"{bytesProgress} | ETA: {eta} | {link.Name}";

            childProgressBar.Tick(transferred, message);
        }

        public static void Dispose()
        {
            foreach (var childProgressBar in ChildrenProgressBars.Values)
            {
                childProgressBar.Dispose();
            }

            _parentProgressBar.Dispose();
        }

        private static string CalculateEta(int transferred, int max)
        {
            // if first call, no eta can be estimated (no data in)
            if (_timeByteBpsStamp.LastTimeStamp == DateTime.MinValue)
            {
                return "";
            }

            var timeDiff = (DateTime.Now - _timeByteBpsStamp.LastTimeStamp).TotalMilliseconds;
            var bytesDiff = transferred - _timeByteBpsStamp.LastBytesStamp;

            var averageBps = _timeByteBpsStamp.AverageBps.Average;

            // if timeDiff > 0 (if method not called to fast)
            if (Math.Abs(timeDiff) > 0.5)
            {
                // TimeSpan.FromSeconds(1).TotalMilliseconds = 1000
                var bps = 1000 / timeDiff * bytesDiff;
                averageBps = _timeByteBpsStamp.AverageBps.ComputeAverage(bps);
            }

            var etaSeconds = (max - transferred) / averageBps;

            // if averageBps unknown (second time but too fast (saving external link)) causing etaSeconds to be NaN
            if (Math.Abs(averageBps) < 1)
            {
                etaSeconds = 0;
            }

            var eta = $"{TimeSpan.FromSeconds(etaSeconds):mm\\:ss}";
            
            return eta;
        }

        private static string FormatSize(int bytes)
        {
            var byteSize = ByteSize.FromBytes(bytes);
            return $"{byteSize.LargestWholeNumberValue:#.000} {byteSize.LargestWholeNumberSymbol}";
        }
    }
}