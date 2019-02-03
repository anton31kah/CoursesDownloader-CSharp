using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.Models;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsFile;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf;
using CoursesDownloader.SharedVariables;
using HtmlAgilityPack;

namespace CoursesDownloader.Downloader.Implementation.ExtractorsAndHandlers
{
    public static class SectionExtractor
    {
        private const string XPathFilterLinksHeadersFolders =
            "//ul[contains(@class, 'weeks') or contains(@class, 'topics')]" + // main ul containing everything (select as parent)
            "//*[self::h1 or self::h2 or self::h3 or self::h4 or self::h5 or self::h6 " + // all headers
            " or self::a[" + // all links that are :
            "   starts-with(@href, 'http://courses.finki.ukim.mk/mod/resource/view.php?id=') " + // normal files
            "or starts-with(@href, 'http://courses.finki.ukim.mk/mod/folder/view.php?id=') " + // or folders
            "or starts-with(@href, 'http://courses.finki.ukim.mk/mod/url/view.php?id=') " + // or urls
            "or starts-with(@href, 'http://courses.finki.ukim.mk/mod/page/view.php?id=') " + // or pages
            "]]"; 

        public static async Task<List<ISection>> ExtractSectionsForCourse(ICourseLink courseLink)
        {
            await CoursesClient.LazyRefresh();
            var coursePageText = await CoursesClient.SessionClient.GetStringAsync(courseLink.Url);
            CoursesClient.FindSessKey(coursePageText);
            var doc = new HtmlDocument();
            doc.LoadHtml(coursePageText);
            var headersLinks = doc.DocumentNode.SelectNodes(XPathFilterLinksHeadersFolders);

            SharedVars.Sections = new List<ISection>();
            var currentSection = new Section();
            SharedVars.Sections.Add(currentSection);

            foreach (var headerLink in headersLinks)
            {
                var itemType = TryGetItemType(headerLink);

                string innerText = null;
                string href = null;

                if (itemType != ItemType.Header)
                {
                    innerText = headerLink.Descendants().First(d => d.Name == "#text").InnerText.DecodeHtml();
                    href = headerLink.Attributes.First(l => l.Name == "href").Value;
                }

                switch (itemType)
                {
                    case ItemType.Header:
                        var headerName = headerLink.InnerText.DecodeHtml();
                        var headerTag = headerLink.OriginalName;
                        var headerId = FindIdFromAncestors(headerLink);
                        currentSection = new Section(new Header(headerName, headerTag, headerId));
                        SharedVars.Sections.Add(currentSection);
                        break;
                    case ItemType.File:
                        currentSection.Links.Add(new FileLink(innerText, href));
                        break;
                    case ItemType.Folder:
                        currentSection.Links.Add(new FolderLink(innerText, href));
                        break;
                    case ItemType.Url:
                        currentSection.Links.Add(new ExternalLink(innerText, href));
                        break;
                    case ItemType.Page:
                        currentSection.Links.Add(new PageLink(innerText, href));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            SharedVars.Sections = SharedVars.Sections.Where(s => s.Links.Any()).ToList();

            return SharedVars.Sections;
        }

        private static string FindIdFromAncestors(HtmlNode htmlElement)
        {
            var anchorId = htmlElement.Attributes.FirstOrDefault(e => e.Name == "id")?.Value;

            while (anchorId.IsNullOrEmpty())
            {
                htmlElement = htmlElement.ParentNode;
                anchorId = htmlElement.Attributes.FirstOrDefault(e => e.Name == "id")?.Value;
            }

            return anchorId;
        }

        private static ItemType TryGetItemType(HtmlNode headerLink)
        {
            var htmlElementTag = headerLink.OriginalName;
            
            if (htmlElementTag.StartsWith("h"))
            {
                return ItemType.Header;
            }
            
            var linkUrl = headerLink.Attributes.First(l => l.Name == "href").Value;

            if (linkUrl.Contains("resource"))
            {
                return ItemType.File;
            }

            if (linkUrl.Contains("folder"))
            {
                return ItemType.Page;
            }

            if (linkUrl.Contains("url"))
            {
                return ItemType.Url;
            }

            if (linkUrl.Contains("page"))
            {
                return ItemType.Page;
            }

            throw new ArgumentOutOfRangeException(nameof(headerLink), "Item of unknown type");
        }

        private enum ItemType
        {
            Header,
            File,
            Folder,
            Url,
            Page
        }
    }
}