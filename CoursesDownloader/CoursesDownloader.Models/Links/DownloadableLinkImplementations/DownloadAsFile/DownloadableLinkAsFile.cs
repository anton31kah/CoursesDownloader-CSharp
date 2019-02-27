using System.IO;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Client.Helpers.HttpClientAutoRedirect;
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
            
            // we request headers because otherwise the file is first put into memory so we lose the whole point of streams
            // since we are using ReadAsStreamAsync, nothing is loaded into memory
            // although we can't use HeadersResponse from previously because that way we can't track progress
            using (var file = await CoursesClient.SessionClient.GetHeadersAsync(DownloadUrl))
            {
                using (var fileStream = File.Create(filename))
                {
                    using (var content = await file.Content.ReadAsStreamAsync())
                    {
                        int totalLength = (int) file.Content.Headers.ContentLength;
                        var totalBytesRead = 0;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var currentRead = await content.ReadAsync(buffer, 0, buffer.Length);
                            if (currentRead == 0)
                            {
                                isMoreToRead = false;
                                DownloadProgressUpdate(totalBytesRead, totalLength);

                                await fileStream.FlushAsync();
                                continue;
                            }

                            await fileStream.WriteAsync(buffer, 0, currentRead);

                            totalBytesRead += currentRead;
                            DownloadProgressUpdate(totalBytesRead, totalLength);

                        } while (isMoreToRead);
                    }
                }
            }
        }
        
        public override async Task GetNameFromUrlNow()
        {
            if (!IsNameFromUrlExtracted)
            {
                IsNameFromUrlExtracted = true;
                string fileName;

                using (var headersResponseResult = await CoursesClient.SessionClient.GetHeadersAsync(DownloadUrl))
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