using System.Collections.Generic;
using System.Linq;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Downloader.Implementation
{
    public static class CoursesDownloaderAutomatic
    {
        public static void Run()
        {
//            ConsoleUtils.DisableStdio();
            HandleMPS();
//            ConsoleUtils.EnableStdio();
        }

        private static void HandleMPS()
        {
            var courseLink = SharedVars.Courses.First(c => c.Name.ToLower().Contains("mps"));
            var sectionsToCheck = new List<string>
            {
                "Втор колоквиум",
                "Lectures - prof. Nevena Ackovska",
                "EN",
                "Лабораториски вежби - Подготовки",
                "Лабораториски вежби 8086/Lab exercises 8086",
                "Колоквиуми и испити (Partial exams and exams)"
            };
            /*
             * so my idea was to for each course to find the sections that you want to download
		     * check if these sections exist, then get the files from there
		     * the files should be checked maybe before even downloading them
		     * check the request content length in bytes and compare it to the file that already exists
		     * here arises the problem, files in the courses don't follow a pattern
		     * they don't follow a pattern in a section let alone the whole course or multiple ones
		     * checking with each file finding if the file exists is a possible solution
		     * but what if two different files have the same size in bytes? nothing can make us sure
		     * that this won't happen, we can't accept the fact that the chance is so tiny, it's still there
		     * but then what if a section changes names? what if it is removed? what if the files are removed?
		     * and how can you match each file on courses with each file you have? the process is very slow
		     * and requires a lot of data so if this script was to run each hour nothing guarantees the speed
		     * nor that courses won't stop working before or even during the process
		     * 267942874 is the total sum of bytes downloaded from courses till 22/11/2018
		     * so halfway through the semester and we have approximately 255.53 megabytes
		     * but 114 is the total number of files downloaded from courses till 22/11/2018
		     * so maybe 114^2 operations is not something slow, it is very fast, still doing that every hour
		     * is not a good idea nor a smart one
		     * because even if it does download everything you need, then what?
		     * how will it organize them? in folders? then what? you reorganize them on your own?
		     * the whole point of this was to avoid organizing then reorganizing the files
		     * grabbing them from folders is even more annoying and takes more time than the old process
		     * of manually downloading the files and putting them where you need them
		     *
		     * so I conclude that this process is automated enough the way it is
		     * find the course then the section and entering a range of 1-7 and downloading all these files
		     * seems pretty automated to me, you have control over everything and at the same time you don't
		     * have to worry about too many details
		     * "it simply works" is what I wanted to achieve creating this project
             */
        }
    }
}