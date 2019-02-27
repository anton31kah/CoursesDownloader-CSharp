using HtmlAgilityPack;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers
{
    public static class LazyHtmlParser
    {
        private static readonly object Lock = new object();
        private static HtmlDocument _document;

        private static HtmlDocument Document
        {
            get
            {
                lock (Lock)
                {
                    return _document;

                }
            }
            set
            {
                lock (Lock)
                {
                    _document = value;
                }
            }
        }

        private static HtmlNode FindUsingXpath(string html, string xpath)
        {
            lock (Lock)
            {
                if (Document == null)
                {
                    Document = new HtmlDocument();
                }

                Document.LoadHtml(html);

                var node = Document.DocumentNode.SelectSingleNode(xpath);

                return node;
            }
        }

        public static string FindUrlWorkaroundInHtml(string html)
        {
            const string xpath = "//div[@class='urlworkaround']/a";
            var node = FindUsingXpath(html, xpath);
            var href = node.Attributes["href"].Value;
            return href;
        }

        public static string FindTitleInHtml(string html)
        {
            const string xpath = "//title";
            var node = FindUsingXpath(html, xpath);
            var title = node.InnerText;
            return title;
        }

        public static string FindShortNameInHtml(string html)
        {
            const string xpathOldCourse = "//div[@class='breadcrumb-nav']/nav/ul/li[5]/span/a/span";
            const string xpathNewCourse = "//div[@class='breadcrumb-nav']/nav/ul/li[3]/span/a/span";
            var node = FindUsingXpath(html, xpathOldCourse) ?? FindUsingXpath(html, xpathNewCourse);

            var shortName = node.InnerText;
            return shortName;
        }
    }
}