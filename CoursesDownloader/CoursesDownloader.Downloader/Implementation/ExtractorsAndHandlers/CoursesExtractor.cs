using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.Models.Links;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers;
using CoursesDownloader.SharedVariables;
using HtmlAgilityPack;

namespace CoursesDownloader.Downloader.Implementation.ExtractorsAndHandlers
{
    public static class CoursesExtractor
    {
        private const string XPathFilterProfileCoursesLinks =
            "//*[self::a[starts-with(@href, 'http://courses.finki.ukim.mk/user/view.php?id=')]]";

        private const string CoursesProfileAllCoursesUrl = "http://courses.finki.ukim.mk/user/profile.php?showallcourses=1";

        private static readonly Dictionary<int, string> SemesterCoursesLongFormNameRegex;

        private static string CurrentSemesterCourseLongFormNameRegex => SemesterCoursesLongFormNameRegex[SharedVars.CurrentSemesterNumber];

        static CoursesExtractor()
        {
            const string winterSuffix = "ZzWwЗз";
            const string summerSuffix = "LlSsЛл";

            SemesterCoursesLongFormNameRegex = new Dictionary<int, string>
            {
                {1, $@"^.{{6,}}2016.{{0,2}}2017.{{0,5}}[{winterSuffix}].*$"},
                {2, $@"^.{{6,}}2016.{{0,2}}2017.{{0,5}}[{summerSuffix}].*$"},
                {3, $@"^.{{6,}}2017.{{0,2}}2018.{{0,5}}[{winterSuffix}].*$"},
                {4, $@"^.{{6,}}2017.{{0,2}}2018.{{0,5}}[{summerSuffix}].*$"},
                {5, $@"^.{{6,}}2018.{{0,2}}2019.{{0,5}}[{winterSuffix}].*$"},
                {6, $@"^.{{6,}}2018.{{0,2}}2019.{{0,5}}[{summerSuffix}].*$"},
                {7, $@"^.{{6,}}2019.{{0,2}}2020.{{0,5}}[{winterSuffix}].*$"},
                {8, $@"^.{{6,}}2019.{{0,2}}2020.{{0,5}}[{summerSuffix}].*$"}
            };
        }

        public static async Task<List<ICourseLink>> ExtractCourses()
        {
            await CoursesClient.LazyRefresh();
            string coursesPageText;
            using (var coursesPage = await CoursesClient.SessionClient.GetAsync(CoursesProfileAllCoursesUrl))
            {
                coursesPageText = await coursesPage.GetTextNow();
            }
            CoursesClient.FindSessKey(coursesPageText);
            var doc = new HtmlDocument();
            doc.LoadHtml(coursesPageText);
            var coursesLinksNodes = doc.DocumentNode.SelectNodes(XPathFilterProfileCoursesLinks);

            var coursesLinksTasks = coursesLinksNodes
                .Where(l => Regex.IsMatch(l.InnerText, CurrentSemesterCourseLongFormNameRegex))
                .Select(async (l, _) =>
                {
                    var longName = ExtractName(l);
                    var url = ExtractHref(l);

                    string shortName;
                    using (var courseHtml = await CoursesClient.SessionClient.GetStreamAsync(url))
                    {
                        shortName = LazyHtmlParser.FindShortNameInHtml(courseHtml);
                        shortName = CleanName(shortName, true);
                    }

                    return new CourseLink($"{shortName,-8}{longName}", url);
                });

            var coursesLinks = await Task.WhenAll(coursesLinksTasks);

            SharedVars.Courses = coursesLinks.ToList<ICourseLink>();
            
            return SharedVars.Courses;
        }

        public static async Task<int> ExtractSemestersCount()
        {
            await CoursesClient.LazyRefresh();
            string profileHtml;
            using (var coursesPage = await CoursesClient.SessionClient.GetAsync(CoursesProfileAllCoursesUrl))
            {
                profileHtml = await coursesPage.GetTextNow();
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(profileHtml);
            var coursesLinks = doc.DocumentNode.SelectNodes(XPathFilterProfileCoursesLinks);

            var semestersCount =
                SemesterCoursesLongFormNameRegex
                    .Reverse()
                    .Select(pair => new
                    {
                        Number = pair.Key,
                        Courses = coursesLinks
                            .Where(node => Regex.IsMatch(node.InnerText, pair.Value))
                    })
                    .First(semester => semester.Courses.Any())
                    .Number;

            return semestersCount;
        }

        private static string ExtractName(HtmlNode l, bool shortName = false)
        {
            return CleanName(l.InnerText, shortName);
        }

        private static string CleanName(string name, bool shortName = false)
        {
            const string nameOnlyRegex = @"(.*?)\s?-?\s?\d{4}.*";

            var courseFullName = name.DecodeHtml();
            var courseCleanName = Regex.Replace(courseFullName, nameOnlyRegex, "$1");
            return shortName 
                ? courseCleanName.TakeWhile(c => c != ' ').Join("") 
                : courseCleanName;
        }

        private static string ExtractHref(HtmlNode l)
        {
            const string fixCourseIdRegex = @"\d+&course=";

            var courseProfileUrl = l.Attributes.First(a => a.Name == "href").Value.DecodeHtml();
            courseProfileUrl = courseProfileUrl.Replace("user", "course");
            courseProfileUrl = Regex.Replace(courseProfileUrl, fixCourseIdRegex, "");
            return courseProfileUrl;
        }
    }
}