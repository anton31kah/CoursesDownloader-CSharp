using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoursesDownloader.Client;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.Models.Links;
using CoursesDownloader.SharedVariables;
using HtmlAgilityPack;
using MoreLinq.Extensions;

namespace CoursesDownloader.Downloader.Implementation.ExtractorsAndHandlers
{
    public static class CoursesExtractor
    {
        private const string XPathFilterCourseLinks =
            "//*[self::a[starts-with(@href, 'http://courses.finki.ukim.mk/course/view.php?id=')]]";

        private static readonly Dictionary<int, string> SemesterCoursesLongFormNameRegex;
        private static readonly Dictionary<int, string> SemesterCoursesShortFormNameRegex;

        private static string CurrentSemesterCourseLongFormNameRegex => SemesterCoursesLongFormNameRegex[CommonVars.CurrentSemesterNumber];
        private static string CurrentSemesterCourseShortFormNameRegex => SemesterCoursesShortFormNameRegex[CommonVars.CurrentSemesterNumber];

        static CoursesExtractor()
        {
            const string winterSuffix = "ZzWwЗз";
            const string summerSuffix = "LlSsЛл";

            SemesterCoursesLongFormNameRegex = new Dictionary<int, string>
            {
                {1, $@"^.{{6,}}2016.{{0,2}}2017.{{0,5}}[{winterSuffix}]$"},
                {2, $@"^.{{6,}}2016.{{0,2}}2017.{{0,5}}[{summerSuffix}]$"},
                {3, $@"^.{{6,}}2017.{{0,2}}2018.{{0,5}}[{winterSuffix}]$"},
                {4, $@"^.{{6,}}2017.{{0,2}}2018.{{0,5}}[{summerSuffix}]$"},
                {5, $@"^.{{6,}}2018.{{0,2}}2019.{{0,5}}[{winterSuffix}]$"},
                {6, $@"^.{{6,}}2018.{{0,2}}2019.{{0,5}}[{summerSuffix}]$"},
                {7, $@"^.{{6,}}2019.{{0,2}}2020.{{0,5}}[{winterSuffix}]$"},
                {8, $@"^.{{6,}}2019.{{0,2}}2020.{{0,5}}[{summerSuffix}]$"}
            };

            SemesterCoursesShortFormNameRegex = new Dictionary<int, string>
            {
                {1, $@"^.{{0,5}}2016.{{0,2}}2017.{{0,5}}[{winterSuffix}].*$"},
                {2, $@"^.{{0,5}}2016.{{0,2}}2017.{{0,5}}[{summerSuffix}].*$"},
                {3, $@"^.{{0,5}}2017.{{0,2}}2018.{{0,5}}[{winterSuffix}].*$"},
                {4, $@"^.{{0,5}}2017.{{0,2}}2018.{{0,5}}[{summerSuffix}].*$"},
                {5, $@"^.{{0,5}}2018.{{0,2}}2019.{{0,5}}[{winterSuffix}].*$"},
                {6, $@"^.{{0,5}}2018.{{0,2}}2019.{{0,5}}[{summerSuffix}].*$"},
                {7, $@"^.{{0,5}}2019.{{0,2}}2020.{{0,5}}[{winterSuffix}].*$"},
                {8, $@"^.{{0,5}}2019.{{0,2}}2020.{{0,5}}[{summerSuffix}].*$"}
            };
        }

        public static async Task<List<ICourseLink>> ExtractCourses()
        {
            await CoursesClient.LazyRefresh();
            string coursesPageText;
            using (var coursesPage = await CoursesClient.SessionClient.GetAsync(CoursesClient.SessionClient.BaseAddress))
            {
                coursesPageText = await coursesPage.GetTextNow();
            }
            CoursesClient.FindSessKey(coursesPageText);
            var doc = new HtmlDocument();
            doc.LoadHtml(coursesPageText);
            var coursesLinks = doc.DocumentNode.SelectNodes(XPathFilterCourseLinks);

            var currentSemesterCourseLongFormNameRegex = CurrentSemesterCourseLongFormNameRegex;
            var currentSemesterCourseShortFormNameRegex = CurrentSemesterCourseShortFormNameRegex;

            string ExtractName(HtmlNode l) => l.Descendants().First(d => d.Name == "#text").InnerText
                                               .DecodeHtml().TakeWhile(c => c != '-').Join("");

            string ExtractHref(HtmlNode l) => l.Attributes.First(a => a.Name == "href").Value;

            // TL;DR extract long names and short names, and join them
            // LONG: extract long names, distinct by URL, same for short names, then join on URL,
            //       then put short with left padding, then put long, names, and finally toList
            CommonVars.Courses = 
                coursesLinks
                    .Where(l => Regex.IsMatch(l.InnerText, currentSemesterCourseLongFormNameRegex))
                    .Select(l => new
                    {
                        Name = ExtractName(l),
                        URL = ExtractHref(l)
                    })
                    .DistinctBy(l => l.URL)
                    .Join(
                        coursesLinks
                            .Where(l => Regex.IsMatch(l.InnerText, currentSemesterCourseShortFormNameRegex))
                            .Select(l => new
                            {
                                Name = ExtractName(l),
                                URL = ExtractHref(l)
                            })
                            .DistinctBy(l => l.URL),
                        longFormLink => longFormLink.URL,
                        shortFormLink => shortFormLink.URL, 
                        (longFormLink, shortFormLink) => 
                            new CourseLink(
                                $"{shortFormLink.Name, -8}{longFormLink.Name}", 
                                     longFormLink.URL
                                )
                        )
                .ToList<ICourseLink>();

            return CommonVars.Courses;
        }
    }
}