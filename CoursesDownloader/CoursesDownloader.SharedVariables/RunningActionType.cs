namespace CoursesDownloader.SharedVariables
{
    public enum RunningActionType
    {
        AskForCourse = 0,
        AskForSection = 1,
        AskForMultipleLinks = 2,
        AskForNamingMethod = 3,
        DownloadSelectedLinks = 4,
        Repeat = 5,
        End = 6
    }
}