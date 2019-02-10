using System.IO;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Client.Helpers;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsShortcut;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf
{
    public class ExternalLink : DownloadableLink, IExternalLink
    {
        private bool AreExternalUrlAndTitleExtracted { get; set; }
        private static string ExternalUrl { get; set; }
        private string Title { get; set; }

        public ExternalLink(string name = "", string url = "", ISection parentSection = null) : base(name, url, parentSection)
        {
        }

        private async Task ExtractExternalUrlAndTitle()
        {
            await CoursesClient.LazyRefresh();

            // Go to url as clicked on courses
            using (var coursesResponse = await CoursesClient.SessionClient.GetHeadersAsyncHttp(Url))
            {
                var externalResponse = coursesResponse;

                // if redirected to courses, grab url workaround, get title from there
                if (coursesResponse.RequestMessage.RequestUri.Host == CoursesClient.SessionClient.BaseAddress.Host)
                {
                    using (var coursesHtml = await coursesResponse.Content.ReadAsStreamAsync())
                    {
                        var urlWorkaround = LazyHtmlParser.FindUrlWorkaroundInHtml(coursesHtml);
                        ExternalUrl = urlWorkaround;

                        // Go to url as clicked on workaround url
                        externalResponse = await CoursesClient.SessionClient.GetHeadersAsyncHttp(urlWorkaround);
                    }
                }
                // else redirected to external link, just save it
                else
                {
                    var url = coursesResponse.RequestMessage.RequestUri.ToString();
                    ExternalUrl = url;
                }

                using (externalResponse)
                {
                    using (var externalHtml = await externalResponse.Content.ReadAsStreamAsync())
                    {
                        var title = LazyHtmlParser.FindTitleInHtml(externalHtml);
                        Title = title;

                        AreExternalUrlAndTitleExtracted = true;
                    }
                }
            }
        }
        
        protected override async Task GetAndSaveFile(string filename)
        {
            var progressEventArgs = new HttpProgressEventArgs(0, null, 0, ExternalUrl.Length);

            DownloadProgressTracker(this, progressEventArgs);

            using (var textWriter = new StreamWriter(filename))
            {
                var fileContent = ShortcutFileHelper.FromTitleAndUrl(Title, ExternalUrl);
                await textWriter.WriteAsync(fileContent);

                progressEventArgs = new HttpProgressEventArgs(100, null, fileContent.Length, fileContent.Length);
                DownloadProgressTracker(this, progressEventArgs);
            }
        }
        
        public override async Task GetNameFromUrlNow()
        {
            if (!AreExternalUrlAndTitleExtracted)
            {
                await ExtractExternalUrlAndTitle();

                FileSize = ExternalUrl.Length;
                Title = Title.DecodeHtml();
                
                FileFromCourses.FileNameOnly = Name;
                FileFromUrl.FileNameOnly = Title;
                FileFromCourses.FileExtensionOnly = FileFromUrl.FileExtensionOnly = ShortcutFileHelper.GetExtension();
            }
        }
    }
}