using System.Collections.Generic;

namespace CoursesDownloader.IModels.ILinks
{
    public interface ICourseLink : ILink
    {
        IList<ISection> Sections { get; }

        ISection this[int key] { get; }
        ISection this[string key] { get; }
        int Count { get; }

        bool Contains(ISection section);
    }
}