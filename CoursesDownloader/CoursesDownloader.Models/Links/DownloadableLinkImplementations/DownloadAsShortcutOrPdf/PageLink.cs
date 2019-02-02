using System.Net.Http.Handlers;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcut.Helpers;
using HtmlAgilityPack;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf
{
    public class PageLink : DownloadableLink, IPageLink
    {
        private bool IsTitleExtracted { get; set; }
        private string Title { get; set; }


        public PageLink(string name = "", string url = "") : base(name, url)
        {
        }

        private async Task ExtractExternalUrlAndTitle()
        {
            await CoursesClient.LazyRefresh();
            
            using (var html = await CoursesClient.SessionClient.GetStreamAsync(Url))
            {
                var title = LazyHtmlParser.FindTitleInHtml(html);
                Title = title;

                IsTitleExtracted = true;
            }
        }
        
        protected override async Task GetAndSaveFile(string filename)
        {
            await CoursesClient.LazyRefresh();

            ReportProgress(0, 1024);

            var html = await ExtractMainHtml();

            ReportProgress(html.Length / 2.0, html.Length);

            var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            
            ReportProgress(pdf.FileSize * 0.9, pdf.FileSize);

            pdf.Save(filename);

            ReportProgress(pdf.FileSize, pdf.FileSize);
        }

        private async Task<string> ExtractMainHtml()
        {
            var html = await CoursesClient.SessionClient.GetStringAsync(Url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var mainNode = doc.DocumentNode.SelectSingleNode("//div[@role = 'main']");
            html = mainNode.OuterHtml;

            return html;
        }

        private void ReportProgress(double done, double max)
        {
            var progressEventArgs = new HttpProgressEventArgs((int) done, null, (long) done, (long?) max);
            DownloadProgressTracker(this, progressEventArgs);
        }

        public override async Task GetNameFromUrlNow()
        {
            if (!IsTitleExtracted)
            {
                await ExtractExternalUrlAndTitle();
                
                Title = Title.DecodeHtml();

                FileFromCourses.FileNameOnly = Name;
                FileFromUrl.FileNameOnly = Title;
                FileFromCourses.FileExtensionOnly = FileFromUrl.FileExtensionOnly = "pdf";
            }
        }
    }
}