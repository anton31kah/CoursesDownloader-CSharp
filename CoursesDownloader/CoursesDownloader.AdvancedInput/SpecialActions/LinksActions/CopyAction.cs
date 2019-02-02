using System.Collections.Generic;
using CoursesDownloader.Common.ExtensionMethods;
using TextCopy;

namespace CoursesDownloader.AdvancedIO.SpecialActions.LinksActions
{
    public class CopyAction : LinksBaseAction
    {
        protected override string Type => "Copy";
        protected override string Description => "Copies whatever you last selected into clipboard";

        protected override void HandleLink(string link)
        {
            Clipboard.SetText(link);
        }

        protected override void HandleMultipleLinks(IEnumerable<string> links)
        {
            HandleLink(links.Join());
        }
    }
}