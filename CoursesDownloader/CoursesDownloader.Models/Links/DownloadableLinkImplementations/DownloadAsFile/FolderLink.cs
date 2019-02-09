using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks.IDownloadableLinkImplementations.IDownloadAsFile;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsFile
{
    public class FolderLink : DownloadableLinkAsFile, IFolderLink
    {
        private const string FolderDownloadLink = "http://courses.finki.ukim.mk/mod/folder/download_folder.php";

        public FolderLink(string name = "", string url = "", ISection parentSection = null) : base(name, url, parentSection)
        {
            var id = Regex.Match(Url, @"\d+$").Value;
            var sessKey = SharedVars.SessKey;
            var data = new Dictionary<string, string>
            {
                {"id", id},
                {"sesskey", sessKey}
            };
            using (var dataContent = new FormUrlEncodedContent(data.ToArray()))
            {
                var dataContentParams = dataContent.ReadAsStringAsync().Result;
                var fullRequestUrl = $"{FolderDownloadLink}?{dataContentParams}";
                DownloadUrl = fullRequestUrl;
            }
        }
    }
}