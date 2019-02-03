using System;
using System.Threading.Tasks;
using CoursesDownloader.Downloader.Implementation;

namespace CoursesDownloader.Downloader
{
    public static class CoursesDownloader
    {
        public static async Task Main()
        {
            // handle console close event, to log out temp user and destroy credentials
            Console.CancelKeyPress += Dispose;
            AppDomain.CurrentDomain.ProcessExit += Dispose;

            await CoursesDownloaderManual.Init();
            await CoursesDownloaderManual.Run();
        }

        private static void Dispose(object sender, EventArgs e)
        {
            Dispose();
        }

        private static void Dispose()
        {
            CoursesDownloaderManual.Dispose();
        }
    }
}
