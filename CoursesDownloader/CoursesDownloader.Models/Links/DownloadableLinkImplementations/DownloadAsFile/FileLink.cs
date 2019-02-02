using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsFile;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsFile
{
    public class FileLink : DownloadableLinkAsFile, IFileLink
    {
        public FileLink(string name = "", string url = "") : base(name, url)
        {
            DownloadUrl = Url;
        }
    }
}