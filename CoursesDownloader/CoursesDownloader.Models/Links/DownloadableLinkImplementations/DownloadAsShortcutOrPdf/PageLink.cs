using System.IO;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Client.Helpers;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsShortcut;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers;
using DinkToPdf;
using HtmlAgilityPack;

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
            
            using (var html = await CoursesClient.SessionClient.GetStreamAsyncHttp(Url))
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

            var pdf = GeneratePdf(html);

            ReportProgress(pdf.Length * 0.9, pdf.Length);

            File.WriteAllBytes(filename, pdf);

            ReportProgress(pdf.Length, pdf.Length);
        }

        private async Task<string> ExtractMainHtml()
        {
            var html = await CoursesClient.SessionClient.GetStringAsyncHttp(Url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var mainNode = doc.DocumentNode.SelectSingleNode("//div[@role = 'main']");
            html = mainNode.OuterHtml;

            return html;
        }

        private byte[] GeneratePdf(string html)
        {
            byte[] pdfBytes;
            using (var pdfTools = new PdfTools())
            {
                var converter = new BasicConverter(pdfTools);

                var doc = new HtmlToPdfDocument
                {
                    GlobalSettings =
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4
                    },
                    Objects =
                    {
                        new ObjectSettings
                        {
                            PagesCount = true,
                            HtmlContent = html,
                            WebSettings = {DefaultEncoding = "utf-8"}
                        }
                    }
                };

                pdfBytes = converter.Convert(doc);
            }

            return pdfBytes;
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