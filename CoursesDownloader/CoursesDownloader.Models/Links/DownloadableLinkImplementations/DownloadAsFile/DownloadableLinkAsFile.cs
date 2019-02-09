using System.IO;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Client.Helpers;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsFile;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsFile
{
    public abstract class DownloadableLinkAsFile : DownloadableLink, IDownloadableLinkAsFile
    {
        protected string DownloadUrl { private get; set; }

        protected DownloadableLinkAsFile(string name = "", string url = "", ISection parentSection = null) : base(name, url, parentSection)
        {
        }
        
        protected override async Task GetAndSaveFile(string filename)
        {
            await CoursesClient.LazyRefresh();

            CoursesClient.AddEvent(DownloadProgressTracker);

            // we request headers because otherwise the file is first put into memory so we lose the whole point of streams
            // since we are using ReadAsStreamAsync, nothing is loaded into memory
            // although we can't use HeadersResponse from previously because that way we can't track progress
            using (var file = await CoursesClient.SessionClient.GetHeadersAsyncHttp(DownloadUrl))
            {
                using (var fileStream = File.Create(filename))
                {
                    using (var content = await file.Content.ReadAsStreamAsync())
                    {
                        await content.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }
                }
            }

            CoursesClient.RemoveEvent(DownloadProgressTracker);
        }
        
        public override async Task GetNameFromUrlNow()
        {
            if (!IsNameFromUrlExtracted)
            {
                IsNameFromUrlExtracted = true;
                string fileName;

                using (var headersResponseResult = await CoursesClient.SessionClient.GetHeadersAsyncHttp(DownloadUrl))
                {
                    fileName = headersResponseResult.Content.Headers.ContentDisposition.FileName
                        .DecodeUtf8()
                        .EscapeQuotes();
                    FileSize = headersResponseResult.Content.Headers.ContentLength ?? 0;
                }

                FileFromUrl.FileNameAndExtensionOnly = fileName;
                FileFromCourses.FileNameOnly = Name;
                FileFromCourses.FileExtensionOnly = FileFromUrl.FileExtensionOnly;
            }
        }
    }
}