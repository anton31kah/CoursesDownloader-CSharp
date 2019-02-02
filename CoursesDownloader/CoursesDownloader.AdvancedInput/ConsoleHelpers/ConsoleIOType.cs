using System;

namespace CoursesDownloader.AdvancedIO.ConsoleHelpers
{
    public enum ConsoleIOType
    {
        BreadCrumbs = ConsoleColor.DarkGray,
        Question = ConsoleColor.Gray,
        Options = ConsoleColor.White,
        RecommendedOption = ConsoleColor.Green,
        YesNoQuestion = ConsoleColor.Yellow,
        Input = ConsoleColor.Cyan,
        Error = ConsoleColor.Red
    }
}