using System.Collections.Generic;

namespace CoursesDownloader.AdvancedIO.SpecialActions.LinksActions
{
    public class OpenAction : LinksBaseAction
    {
        protected override string Type => "Open";
        protected override string Description => "Opens whatever you last selected in your browser";

        protected override void HandleLink(string link)
        {
            System.Diagnostics.Process.Start(link);
        }

        protected override void HandleMultipleLinks(IEnumerable<string> links)
        {
            foreach (var link in links)
            {
                HandleLink(link);
            }
        }
    }
}