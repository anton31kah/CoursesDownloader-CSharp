using System.Collections.Generic;
using System.Linq;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;

namespace CoursesDownloader.SharedVariables
{
    public static class SharedVars
    {
        private static readonly object Lock = new object();
        
        #region Backing Fields

        private static Queue<RunningActionType> _previousRunningActionTypes;
        private static RunningActionType _currentRunningActionType;
        private static List<ICourseLink> _courses;
        private static List<ISection> _sections;
        private static SortedDictionary<RunningActionType, string> _chosenItemsTillNow;
        private static ICourseLink _selectedCourseLink;
        private static ISection _selectedSection;
        private static List<IDownloadableLink> _selectedLinks;
        private static string _sessKey;
        private static NamingMethod _namingMethod = NamingMethod.CoursesName;
        private static int _currentSemesterNumber;

        #endregion

        public static Queue<RunningActionType> PreviousRunningActionTypes
        {
            get
            {
                lock (Lock)
                {
                    _previousRunningActionTypes = _previousRunningActionTypes ?? new Queue<RunningActionType>();
                    return _previousRunningActionTypes;
                }
            }
            private set
            {
                lock (value)
                {
                    _previousRunningActionTypes = _previousRunningActionTypes ?? new Queue<RunningActionType>();
                    _previousRunningActionTypes = value;
                }
            }
        }

        public static RunningActionType CurrentRunningActionType
        {
            get
            {
                lock (Lock)
                {
                    return _currentRunningActionType;
                }
            }
            set
            {
                lock (Lock)
                {
                    EnqueueActionAndClear(value);
                    
                    _currentRunningActionType = value;
                }
            }
        }

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

        public static SortedDictionary<RunningActionType, string> ChosenItemsTillNow
        {
            get
            {
                lock (Lock)
                {
                    _chosenItemsTillNow = _chosenItemsTillNow ?? new SortedDictionary<RunningActionType, string>();
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

        public static int CurrentSemesterNumber
        {
            get
            {
                lock (Lock)
                {
                    return _currentSemesterNumber;
                }
            }
            set
            {
                lock (Lock)
                {
                    _currentSemesterNumber = value;
                }
            }
        }


        /// <summary>
        /// At the end of this method, PreviousRunningActionTypes will have the last running range only
        /// </summary>
        /// <param name="value"></param>
        private static void EnqueueActionAndClear(RunningActionType value)
        {
            // enqueue, if queue overloads, dequeue
            PreviousRunningActionTypes.Enqueue(value);
            if (PreviousRunningActionTypes.Count > 10)
            {
                PreviousRunningActionTypes.Dequeue();
            }

            // clear any actions from previous "sessions" of runs (so only the most recent "session" of actions is stored)
            var toRemove = PreviousRunningActionTypes.ToList().FindLastIndex(i => i >= RunningActionType.Repeat);
            for (var _ = 0; _ <= toRemove; _++)
            {
                PreviousRunningActionTypes.Dequeue();
            }

            // if previous sessions were going back and forth, then keep the most recent ascending slice
            var runningActionTypes = PreviousRunningActionTypes
                .Reverse()
                .TakeWhile(c => c > RunningActionType.AskForCourse)
                .Concat(
                    PreviousRunningActionTypes
                        .Reverse()
                        .SkipWhile(c => c > RunningActionType.AskForCourse)
                        .Take(1)
                )
                .Reverse();

            PreviousRunningActionTypes = new Queue<RunningActionType>(runningActionTypes);

        }

    }
}
