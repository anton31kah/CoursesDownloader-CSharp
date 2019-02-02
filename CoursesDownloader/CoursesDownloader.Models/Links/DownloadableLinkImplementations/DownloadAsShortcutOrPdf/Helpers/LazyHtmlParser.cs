using System;
using System.IO;
using System.Text;
using CoursesDownloader.Common.ExtensionMethods;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcut.Helpers
{
    public static class LazyHtmlParser
    {
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

        private static string FindTextBetween(Stream stream, string startSeparator, string endSeparator)
        {
            var startQueue = new LimitedQueue<char>(startSeparator.Length);
            var endQueue = new LimitedQueue<char>(endSeparator.Length);
            var text = new StringBuilder();

            var state = 0;
            // 0 searching for start
            // 1 start found, reading text, searching for end

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var c = (char)reader.Read();

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
                                return text.ToString(0, text.Length - endSeparator.Length);
                            }

                            break;
                        }
                    }
                }
            }

            throw new IndexOutOfRangeException($"Couldn't find {startSeparator} anywhere in the stream");
        }
    }
}