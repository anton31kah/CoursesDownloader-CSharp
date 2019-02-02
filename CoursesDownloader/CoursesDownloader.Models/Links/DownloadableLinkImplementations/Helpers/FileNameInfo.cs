using System.IO;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IHelpers;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.Helpers
{
    public class FileNameInfo : IFileNameInfo
    {
        private string _filePathOnly;
        private string _fileNameOnly;
        private string _fileExtensionOnly;
        private string _fileNameAndExtensionOnly;
        private string _fullPathAndFileAndExtension;

        /// <summary>
        /// Only path without name nor extension
        /// </summary>
        public string FilePathOnly
        {
            get => _filePathOnly;
            set
            {
                _filePathOnly = value;

                var (_, name, ext) = Split();
                _fullPathAndFileAndExtension = Path.Combine(_filePathOnly, $"{name}{ext}");
            }
        }

        /// <summary>
        /// Only name without path nor extension
        /// </summary>
        public string FileNameOnly
        {
            get => _fileNameOnly;
            set
            {
                value = FileNameHelpers.FixInvalidFileNameChars(value);
                _fileNameOnly = value;

//                var (dir, _, ext) = Split();
                _fileNameAndExtensionOnly = $"{_fileNameOnly}{_fileExtensionOnly}";
                _fullPathAndFileAndExtension = Path.Combine(_filePathOnly, $"{_fileNameOnly}{_fileExtensionOnly}");
            }
        }

        /// <summary>
        /// Only extension without path nor name
        /// </summary>
        public string FileExtensionOnly
        {
            get => _fileExtensionOnly;
            set
            {
                _fileExtensionOnly = value;
                if (!_fileExtensionOnly.StartsWith("."))
                {
                    _fileExtensionOnly = $".{_fileExtensionOnly}";
                }

//                var (dir, name, _) = Split();
                _fileNameAndExtensionOnly = $"{_fileNameOnly}{_fileExtensionOnly}";
                _fullPathAndFileAndExtension = Path.Combine(_filePathOnly, $"{_fileNameOnly}{_fileExtensionOnly}");
            }
        }

        /// <summary>
        /// Only name.extension without path
        /// </summary>
        public string FileNameAndExtensionOnly
        {
            get => _fileNameAndExtensionOnly;
            set
            {
                _fileNameAndExtensionOnly = value;

                var (_, name, ext) = Split(_fileNameAndExtensionOnly);
                name = FileNameHelpers.FixInvalidFileNameChars(name);
                _fileNameOnly = name;
                _fileExtensionOnly = ext;
                _fullPathAndFileAndExtension = Path.Combine(_filePathOnly, $"{name}{ext}");
            }
        }

        /// <summary>
        /// Full path/name.extension
        /// </summary>
        public string FullPathAndFileAndExtension
        {
            get => _fullPathAndFileAndExtension;
            set
            {
                _fullPathAndFileAndExtension = value;

                var (dir, name, ext) = Split(_fullPathAndFileAndExtension);
                name = FileNameHelpers.FixInvalidFileNameChars(name);
                _filePathOnly = dir;
                _fileNameOnly = name;
                _fileExtensionOnly = ext;
                _fileNameAndExtensionOnly = $"{name}{ext}";
            }
        }

        public FileNameInfo()
        {
            _filePathOnly = "";
            _fileNameOnly = "";
            _fileExtensionOnly = "";
            _fileNameAndExtensionOnly = "";
            _fullPathAndFileAndExtension = "";
        }

        private (string dir, string name, string ext) Split(string path = null)
        {
            path = path ?? _fullPathAndFileAndExtension;
            
            var fileParent = path.IfIsNotNullNorEmpty(() => Path.GetDirectoryName(path));
            var filename = Path.GetFileNameWithoutExtension(path);
            var fileType = Path.GetExtension(path);

            return (fileParent, filename, fileType);
        }
    }
}