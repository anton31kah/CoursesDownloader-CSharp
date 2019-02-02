using System;
using System.IO;
using CoursesDownloader.Common.ExtensionMethods;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers
{
    public static class FileNameHelpers
    {
        private static readonly string DownloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        public static string FullyPrepare(string filename)
        {
            filename = FixInvalidFileNameChars(filename);
            filename = PrependDownloadsPath(filename);
            filename = FixIfDuplicate(filename);

            return filename;
        }

        public static string FixInvalidFileNameChars(string filename)
        {
            return filename.Replace(Path.GetInvalidFileNameChars(), "_");
        }

        private static string PrependDownloadsPath(string filename)
        {
            return Path.Combine(DownloadsPath, filename);
        }

        private static string FixIfDuplicate(string filepath)
        {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var fileType = Path.GetExtension(filepath);
            var fileParent = Path.GetDirectoryName(filepath) ?? "";

            var i = 1;
            var newFilename = filename;

            var path = Path.Combine(fileParent, newFilename + fileType);

            while (File.Exists(path))
            {
                newFilename = $"{filename}_{i}";
                i++;

                path = Path.Combine(fileParent, newFilename + fileType);
            }

            return path;
        }
    }
}