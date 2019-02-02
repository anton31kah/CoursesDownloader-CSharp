using System.Threading.Tasks;
using CoursesDownloader.Downloader.Implementation;

namespace CoursesDownloader.Downloader
{
    public static class CoursesDownloader
    {
        public static async Task Main()
        {
            await CoursesDownloaderManual.Init();
            await CoursesDownloaderManual.Run();
        }
    }
}
