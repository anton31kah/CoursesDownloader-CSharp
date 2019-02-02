namespace CoursesDownloader.IModels.ILinks
{
    public interface ILink
    {
        string Name { get; }
        string Url { get; }

        string ToString();
    }
}