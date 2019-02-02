using CoursesDownloader.IModels.ILinks;

namespace CoursesDownloader.Models.Links
{
    public class CourseLink : Link, ICourseLink
    {
        public CourseLink(string name = "", string url = "") : base(name, url)
        {
        }
    }
}