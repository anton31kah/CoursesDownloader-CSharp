using System;
using System.IO;
using System.Text;
using CoursesDownloader.Common.ExtensionMethods;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers
{
    public static class LazyHtmlParser
    {
        private static readonly object Lock = new object();
        private static string _restOfLine;

        private static string RestOfLine
        {
            get
            {
                lock (Lock)
                {
                    return _restOfLine;
                }
            }
            set
            {
                lock (Lock)
                {
                    _restOfLine = value;
                }
            }
        }

        public static string FindUrlWorkaroundInHtml(Stream stream)
        {
            const string startSeparator = "Click <a href=\"";
            const string endSeparator = "\" >";

            return FindTextBetween(stream, startSeparator, endSeparator);
        }

        public static string FindTitleInHtml(Stream stream)
        {
            const string startSeparator = "<title>";
            const string endSeparator = "</title>";

            return FindTextBetween(stream, startSeparator, endSeparator);
        }

        public static string FindShortNameInHtml(Stream stream)
        {
            const string startSeparator = "itemprop=\"title\">";
            const string endSeparator = "</span>";

            // run twice because the first run will return "Home"
            // the 4 links that appear are "Home", "{CourseShortName}", "Participants", "{UserName}"
            // or
            // "Home", "Courses", "FINKI", "{Semester}", "{CourseShortName}", "Participants", "{UserName}"

            RestOfLine = null;
            var foundText = FindTextBetween(stream, startSeparator, endSeparator, 2);
            if (foundText == "Courses")
            {
                foundText = FindTextBetween(stream, startSeparator, endSeparator, 3);
            }

            RestOfLine = null;
            return foundText;
        }

        private static string FindTextBetween(Stream stream, string startSeparator, string endSeparator, int numberOfAppearances = 1)
        {
            var startQueue = new LimitedQueue<char>(startSeparator.Length);
            var endQueue = new LimitedQueue<char>(endSeparator.Length);
            var text = new StringBuilder();

            var state = 0;
            // 0 searching for start
            // 1 start found, reading text, searching for end

            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                string line;

                var readNewLine = RestOfLine == null; // read new line if restOfLine is empty, if not empty, read old line

                while ((line = readNewLine ? reader.ReadLine() : RestOfLine) != null)
                {
                    readNewLine = true;
                    RestOfLine = null;
                    var usedChars = 0;

                    foreach (var c in line)
                    {
                        usedChars++;

                        switch (state)
                        {
                            case 0:
                            {
                                startQueue.Enqueue(c);
                                if (startQueue.Join("") == startSeparator)
                                {
                                    state++;
                                }

                                break;
                            }
                            case 1:
                            {
                                text.Append(c);
                                endQueue.Enqueue(c);
                                if (endQueue.Join("") == endSeparator)
                                {
                                    numberOfAppearances--;
                                    if (numberOfAppearances > 0)
                                    {
                                        state--;
                                        startQueue.Clear();
                                        endQueue.Clear();
                                        text.Clear();

                                        continue;
                                    }

                                    RestOfLine = line.Substring(usedChars);
                                    return text.ToString(0, text.Length - endSeparator.Length);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            throw new IndexOutOfRangeException($"Couldn't find {startSeparator} anywhere in the stream");
        }
    }
}