using System.Collections.Generic;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;

namespace CoursesDownloader.SharedVariables
{
    public static class CommonVars
    {
        private static readonly object Lock = new object();

        #region Backing Fields

        private static List<ICourseLink> _courses;
        private static List<ISection> _sections;
        private static Dictionary<string, string> _chosenItemsTillNow;
        private static ICourseLink _selectedCourseLink;
        private static ISection _selectedSection;
        private static List<IDownloadableLink> _selectedLinks;
        private static string _sessKey;
        private static NamingMethod _namingMethod = NamingMethod.CoursesName;

        #endregion

        public static List<ICourseLink> Courses
        {
            get
            {
                lock (Lock)
                {
                    return _courses;
                }
            }
            set
            {
                lock (Lock)
                {
                    _courses = value;
                }
            }
        }

        public static List<ISection> Sections
        {
            get
            {
                lock (Lock)
                {
                    _sections = _sections ?? new List<ISection>();
                    return _sections;
                }

            }
            set
            {
                lock (Lock)
                {
                    _sections = value;
                }
            }
        }

        public static Dictionary<string, string> ChosenItemsTillNow
        {
            get
            {
                lock (Lock)
                {
                    _chosenItemsTillNow = _chosenItemsTillNow ?? new Dictionary<string, string>();
                    return _chosenItemsTillNow;
                }
            }
        }

        public static ICourseLink SelectedCourseLink
        {
            get
            {
                lock (Lock)
                {
                    return _selectedCourseLink;
                }
            }
            set
            {
                lock (Lock)
                {
                    _selectedCourseLink = value;
                }
            }
        }

        public static ISection SelectedSection
        {
            get
            {
                lock (Lock)
                {
                    return _selectedSection;
                }
            }
            set
            {
                lock (Lock)
                {
                    _selectedSection = value;
                }
            }
        }

        public static List<IDownloadableLink> DownloadQueue
        {
            get
            {
                lock (Lock)
                {
                    _selectedLinks = _selectedLinks ?? new List<IDownloadableLink>();
                    return _selectedLinks;
                }
            }
        }
        
        public static string SessKey
        {
            get
            {
                lock (Lock)
                {
                    return _sessKey;
                }
            }
            set
            {
                lock (Lock)
                {
                    _sessKey = value;
                }
            }
        }

        public static NamingMethod NamingMethod
        {
            get
            {
                lock (Lock)
                {
                    return _namingMethod;
                }
            }
            set
            {
                lock (Lock)
                {
                    _namingMethod = value;
                }
            }
        }

        private const int CurrentSemesterNumberDefault = 5;

        public static int CurrentSemesterNumber { get; set; } = CurrentSemesterNumberDefault;

    }
}
