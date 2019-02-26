using System.IO;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Client.Helpers;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels;
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
        
        public PageLink(string name = "", string url = "", ISection parentSection = null) : base(name, url, parentSection)
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

            DownloadProgressUpdate(0, 1024);

            var html = await ExtractMainHtml();

            DownloadProgressUpdate(html.Length / 2.0, html.Length);

            var pdf = GeneratePdf(html);

            DownloadProgressUpdate(pdf.Length * 0.9, pdf.Length);

            File.WriteAllBytes(filename, pdf);

            DownloadProgressUpdate(pdf.Length, pdf.Length);
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

            var pdfBytes = PdfConvertorSingleton.Instance.Converter.Convert(doc);

            return pdfBytes;
        }

        private sealed class PdfConvertorSingleton
        {
            static PdfConvertorSingleton()
            {
            }

            private PdfConvertorSingleton()
            {
                Converter = new SynchronizedConverter(new PdfTools());
            }

            public static PdfConvertorSingleton Instance { get; } = new PdfConvertorSingleton();

            public SynchronizedConverter Converter { get; }
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