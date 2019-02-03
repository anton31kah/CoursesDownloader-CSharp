using System.Runtime.InteropServices;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers
{
    public static class CrossPlatformShortcutFileHelper
    {
        public static string FromTitleAndUrl(string title, string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "[InternetShortcut]\n" +
                       $"URL={url}";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "[Desktop Entry]\n" +
                       "Encoding=UTF-8\n" +
                       $"Name=Link to {title}\n" +
                       "Type=Link\n" +
                       $"URL={url}\n" +
                       "Icon=text-html";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                       "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n" +
                       "<plist version=\"1.0\">\n" +
                       "<dict>\n" +
                       "    <key>URL</key>\n" +
                       $"    <string>{url}</string>\n" +
                       "</dict>\n" +
                       "</plist>";
            }

            // cross platform html
            return "<html>\n" +
                   "<head>\n" +
                   $"    <meta http-equiv=\"refresh\" content=\"0; url={url}\">\n" +
                   "</head>\n" +
                   "</html>";
        }
        public static string GetExtension()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "url";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "desktop";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "webloc";
            }
            return "html";
        }
    }
}