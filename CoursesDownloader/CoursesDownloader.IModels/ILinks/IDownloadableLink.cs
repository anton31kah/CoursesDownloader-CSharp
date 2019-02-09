using System.Threading.Tasks;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IHelpers;

namespace CoursesDownloader.IModels.ILinks
{
    public interface IDownloadableLink : ILink
    {
        ISection ParentSection { get; }

        IFileNameInfo FileFromCourses { get; }
        IFileNameInfo FileFromUrl { get; }

        long FileSize { get; } // in bytes

        Task Download(string[] middlePath);
        Task GetNameFromUrlNow();
    }
}