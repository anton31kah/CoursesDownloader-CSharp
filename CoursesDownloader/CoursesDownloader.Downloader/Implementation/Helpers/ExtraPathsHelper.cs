using System.Collections.Generic;
using System.Linq;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks;
using CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Downloader.Implementation.Helpers
{
    public static class ExtraPathsHelper
    {
        private static Dictionary<IDownloadableLink, string[]> _extraPathPerLink = new Dictionary<IDownloadableLink, string[]>();

        internal static Dictionary<IDownloadableLink, string[]> FillExtraPaths()
        {
            _extraPathPerLink = new Dictionary<IDownloadableLink, string[]>();

            var differentCourses =
                SharedVars.DownloadQueue
                    .Select(link => link.ParentSection.ParentCourse)
                    .Distinct()
                    .ToList();

            if (differentCourses.Count > 1)
            {
                HandleDifferentCourses(differentCourses);
            }
            else
            {
                HandleDifferentSections(SharedVars.DownloadQueue);
            }

            return _extraPathPerLink;
        }

        private static void HandleDifferentCourses(IEnumerable<ICourseLink> differentCourses)
        {
            foreach (var course in differentCourses)
            {
                var dirName = FileNameHelpers.FullyPrepareDirectory(course.Name.TrimInnerSpaces(), true);

                var linksWithinThisCourse =
                    SharedVars.DownloadQueue
                        .Where(link => link.ParentSection.ParentCourse == course)
                        .ToList();

                AdjustExtraPaths(linksWithinThisCourse, courseDirName: dirName, null);

                HandleDifferentSections(linksWithinThisCourse);
            }
        }

        private static void HandleDifferentSections(ICollection<IDownloadableLink> commonParentLinks)
        {
            var differentSections =
                commonParentLinks
                    .Select(link => link.ParentSection)
                    .Distinct()
                    .ToList();

            if (differentSections.Count > 1)
            {
                foreach (var section in differentSections)
                {
                    var dirName = FileNameHelpers.FullyPrepareDirectory(section.Header.Name, true);
                    
                    var linksWithinThisSection =
                        commonParentLinks
                            .Where(link => link.ParentSection == section)
                            .ToList();

                    AdjustExtraPaths(linksWithinThisSection, null, dirName);
                }
            }
        }

        private static void AdjustExtraPaths(IEnumerable<IDownloadableLink> filteredLinks, string courseDirName = null, string sectionDirName = null)
        {
            foreach (var link in filteredLinks)
            {
                _extraPathPerLink.TryAdd(link, new[] {"", ""});
                
                _extraPathPerLink[link][0] = courseDirName ?? _extraPathPerLink[link][0];
                _extraPathPerLink[link][1] = sectionDirName ?? _extraPathPerLink[link][1];
            }
        }
    }
}