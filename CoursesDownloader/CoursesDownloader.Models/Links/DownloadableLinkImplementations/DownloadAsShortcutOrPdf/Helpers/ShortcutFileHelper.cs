namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcutOrPdf.Helpers
{
    public static class ShortcutFileHelper
    {
        private const string HtmlTemplate = @"
<!DOCTYPE html>

<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <meta http-equiv=""refresh"" content=""0; url={Url}"" />
    <title>{Title}</title>
</head>
<body>
    <p>Click on <a href=""{Url}"">{Url}</a> to redirect manually to the url (if it doesn't redirect automatically).</p>
</body>
</html>
";

        public static string FromTitleAndUrl(string title, string url)
        {
            return HtmlTemplate
                .Replace("{Url}", url)
                .Replace("{Title}", title);
        }

        public static string GetExtension()
        {
            return "html";
        }
    }
}