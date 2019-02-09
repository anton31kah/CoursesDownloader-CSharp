using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsFile;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsFile
{
    public class FileLink : DownloadableLinkAsFile, IFileLink
    {
        public FileLink(string name = "", string url = "", ISection parentSection = null) : base(name, url, parentSection)
        {
            DownloadUrl = Url;
        }
    }
}