using System;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsFile;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsShortcut;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers
{
    public class ExternalUrlIsFileException : Exception
    {
        public IExternalLink ExternalLink { get; }
        public IFileLink FileLink { get; }

        public ExternalUrlIsFileException(IExternalLink externalLink, IFileLink fileLink)
        {
            ExternalLink = externalLink;
            FileLink = fileLink;
        }
    }
}