using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CoursesDownloader.Downloader.Implementation;

namespace CoursesDownloader.Downloader
{
    public static class CoursesDownloader
    {
        #region Trap application termination

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _handler;

        private enum CtrlType
        {
            CtrlCEvent = 0,
            CtrlBreakEvent = 1,
            CtrlCloseEvent = 2,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent = 6
        }

        private static bool Handler(CtrlType sig)
        {
            CoursesDownloaderManual.Dispose();

            Environment.Exit(-1);

            return true;
        }
        #endregion

        public static async Task Main()
        {
            // handle console close event, to log out temp user and destroy credentials
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);


            await CoursesDownloaderManual.Init();
            await CoursesDownloaderManual.Run();
        }
    }
}
