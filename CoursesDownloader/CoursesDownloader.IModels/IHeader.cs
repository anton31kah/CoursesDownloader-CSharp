namespace CoursesDownloader.IModels
{
    public interface IHeader
    {
        ISection ParentSection { get; }

        string Name { get; }
        string Order { get; }
        string AnchorId { get; }

        string ToString();
    }
}