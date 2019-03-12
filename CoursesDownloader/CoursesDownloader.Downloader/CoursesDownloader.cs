using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.Downloader.Implementation;

namespace CoursesDownloader.Downloader
{
    public static class CoursesDownloader
    {
        public static async Task Main()
        {
            // handle console close event, to log out temp user and destroy credentials
            ConsoleUtils.AddCancelKeyPressEvent(DisposeSilently);
            AppDomain.CurrentDomain.UnhandledException += DisposeWithException;
            AppDomain.CurrentDomain.ProcessExit += Dispose;

            try
            {
                await CoursesDownloaderManual.Init();
                await CoursesDownloaderManual.Run();
            }
            finally
            {
                DisposeAndClose();
            }
        }

        private static void HandleUnhandledException(Exception e)
        {
            var postUrl = "https://github.com/anton31kah/CoursesDownloader-CSharp/issues/new" +
                          "?assignees=anton31kah&labels=bug&title=Unhandled+Exception&body=";
            postUrl += e.ToString();
            ConsoleUtils.WriteLine(
                "Unfortunately an exception happened, please post an issue of this here\n" +
                "To start the url enter [yes], to just close enter [no]\n" +
                "Details\n" +
                $"{e}\n\n");
            var answer = ConsoleUtils.ReadLine($"[yes/no]? >>> ", ConsoleIOType.YesNoQuestion);
            if (answer?.StartsWith("y", StringComparison.CurrentCultureIgnoreCase) ?? false)
            {
                Process.Start(postUrl.Replace("\n", "%0D%0A"));
            }
        }

        private static void DisposeWithException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                HandleUnhandledException((Exception) e.ExceptionObject);
            }
            catch
            {
                // Swallow exception
            }
            finally
            {
                DisposeAndClose();
            }
        }

        private static void DisposeSilently(object sender, ConsoleCancelEventArgs e)
        {
            try
            {
                DisposeAndClose();
            }
            catch
            {
                // Swallow exception
            }
        }

        private static void Dispose(object sender, EventArgs e)
        {
            DisposeAndClose();
        }

        private static void DisposeAndClose()
        {
            CoursesDownloaderManual.Dispose();
            Process.GetCurrentProcess().Kill();
        }
    }
}
