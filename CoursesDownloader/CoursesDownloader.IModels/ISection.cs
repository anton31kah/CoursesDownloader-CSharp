using System;
using System.Collections.Generic;
using CoursesDownloader.IModels.ILinks;

namespace CoursesDownloader.IModels
{
    public interface ISection
    {
        IHeader Header { get; }
        IList<IDownloadableLink> Links { get; }
        ICourseLink ParentCourse { get; }

        ILink this[int key] { get; }
        ILink this[string key] { get; }
        int Count { get; }

        bool Contains(ILink link);

        string ToString();
    }
}