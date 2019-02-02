namespace CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IHelpers
{
    public interface IFileNameInfo
    {
        string FilePathOnly { get; set; }
        string FileNameOnly { get; set; }
        string FileExtensionOnly { get; set; }
        string FileNameAndExtensionOnly { get; set; }
        string FullPathAndFileAndExtension { get; set; }
    }
}