using System;
using System.IO;
using CoursesDownloader.Common.ExtensionMethods;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers
{
    public static class FileNameHelpers
    {
        private static readonly string DownloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        public static string FullyPrepareFile(string filename, string[] middlePath)
        {
            filename = FixInvalidFilePathChars(filename);
            filename = PrependDownloadsPath(filename, middlePath);
            filename = FixIfDuplicate(filename);

            return filename;
        }

        public static string FullyPrepareDirectory(string dirName, bool onlyDirName)
        {
            dirName = FixInvalidFilePathChars(dirName);
            dirName = PrependDownloadsPath(dirName);
            dirName = FixIfDuplicate(dirName);
            dirName = onlyDirName ? Path.GetFileName(dirName) : dirName;

            return dirName;
        }

        public static string FixInvalidFilePathChars(string path)
        {
            return path.Replace(Path.GetInvalidFileNameChars(), "_");
        }

        private static string PrependDownloadsPath(string path, string[] middlePath = null)
        {
            var combinedMiddlePath = "";
            if (middlePath != null)
            {
                combinedMiddlePath = Path.Combine(middlePath);
            }

            return Path.Combine(DownloadsPath, combinedMiddlePath, path);
        }

        private static string FixIfDuplicate(string fileOrDirPath)
        {
            // when dir, fileOrDirName will hold the directory name (for a/b/c/d it'll be d)
            // and type will be empty

            var pathName = Path.GetFileNameWithoutExtension(fileOrDirPath);
            var fileType = Path.GetExtension(fileOrDirPath);
            var pathParent = Path.GetDirectoryName(fileOrDirPath) ?? "";

            var i = 1;
            var newPath = pathName;

            var path = Path.Combine(pathParent, newPath + fileType);

            while (File.Exists(path))
            {
                newPath = $"{pathName}_{i}";
                i++;

                path = Path.Combine(pathParent, newPath + fileType);
            }

            return path;
        }
    }
}