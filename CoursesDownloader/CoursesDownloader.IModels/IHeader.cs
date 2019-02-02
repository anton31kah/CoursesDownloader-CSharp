namespace CoursesDownloader.IModels
{
    public interface IHeader
    {
        string Name { get; }
        string Order { get; }
        string AnchorId { get; }

        string ToString();
    }
}