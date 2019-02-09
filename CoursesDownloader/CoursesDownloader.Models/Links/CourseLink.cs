using System.Collections.Generic;
using System.Linq;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;

namespace CoursesDownloader.Models.Links
{
    public class CourseLink : Link, ICourseLink
    {
        public IList<ISection> Sections { get; }

        public ISection this[int key] => Sections[key];
        public ISection this[string key] => Sections.First(s => s.Header.Name == key);
        public int Count => Sections.Count;

        public CourseLink(string name = "", string url = "", List<ISection> sections = null) : base(name, url)
        {
            Sections = sections ?? new List<ISection>();
        }

        public bool Contains(ISection section)
        {
            return Sections.Contains(section);
        }
    }
}