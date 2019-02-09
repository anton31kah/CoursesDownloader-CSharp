using System.IO;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IHelpers;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Models.Links
{
    public abstract class DownloadableLink : Link, IDownloadableLink
    {
        protected bool IsNameFromUrlExtracted { get; set; }

        public ISection ParentSection { get; }

        public IFileNameInfo FileFromCourses { get; }
        public IFileNameInfo FileFromUrl { get; }

        public long FileSize { get; protected set; } // in bytes

        protected DownloadableLink(string name = "", string url = "", ISection parentSection = null) : base(name, url)
        {
            ParentSection = parentSection;
            FileFromCourses = new FileNameInfo();
            FileFromUrl = new FileNameInfo();
        }

        public async Task Download(string[] middlePath)
        {
            await CoursesClient.LazyRefresh();
            await GetNameFromUrlNow();

            string filename;

            switch (SharedVars.NamingMethod)
            {
                case NamingMethod.CoursesName:
                    filename = FileFromCourses.FileNameAndExtensionOnly;
                    break;
                case NamingMethod.UrlName:
                    filename = FileFromUrl.FileNameAndExtensionOnly;
                    break;
                default:
                    filename = FileFromUrl.FileNameAndExtensionOnly;
                    break;
            }

            var filepath = FileNameHelpers.FullyPrepareFile(filename, middlePath);
            FileFromUrl.FullPathAndFileAndExtension = FileFromCourses.FullPathAndFileAndExtension = filepath;

            var fileInfo = new FileInfo(filepath);
            if (!fileInfo.Directory?.Exists ?? false)
            {
                fileInfo.Directory.Create();
            }

            await GetAndSaveFile(filepath);
        }

        protected abstract Task GetAndSaveFile(string filename);

        protected void DownloadProgressTracker(object sender, HttpProgressEventArgs e)
        {
            ProgressBarUtil.TickFile(this, e);
        }

        public abstract Task GetNameFromUrlNow();
    }
}