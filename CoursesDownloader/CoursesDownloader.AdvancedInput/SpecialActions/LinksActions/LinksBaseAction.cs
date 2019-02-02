using System.Collections.Generic;
using System.Linq;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.LinksActions
{
    public class LinksBaseAction : BaseAction
    {
        protected override string Type => "LinksBase";
        protected override string Description => "Handles actions related to links, like Copy and Open";

        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;
            var actionWord = Type.ToLower();
            if (inputString.ToLower().Contains(actionWord))
            {
                IEnumerable<int> sequence = null;
                foreach (var piece in inputString.Split(' '))
                {
                    sequence = MenuChooseItem.ValidateInput(piece);
                    if (sequence != null)
                    {
                        sequence = sequence.Select(i => i - 1).Where(i => i >= 0);
                        break;
                    }
                }

                ConfirmAction($"{actionWord} the last selection",
                    () =>
                    {
                        State = ActionState.FoundAndHandled;
                        var stagesDepth = CommonVars.ChosenItemsTillNow.Count;
                        switch (stagesDepth)
                        {
                            case 0 when sequence == null:
                                HandleLink("https://courses.finki.ukim.mk/");
                                break;
                            case 0 when sequence != null:
                                HandleMultipleLinks(CommonVars.Courses
                                    .Where((_, i) => sequence.Contains(i))
                                    .Select(link => link.Url)
                                );
                                break;
                            case 1 when sequence == null:
                                HandleLink(CommonVars.SelectedCourseLink.Url);
                                break;
                            case 1 when sequence != null:
                                HandleMultipleLinks(CommonVars.Sections
                                    .Where((_, i) => sequence.Contains(i))
                                    .Select(section => section.Header.AnchorId)
                                    .Select(id => $"{CommonVars.SelectedCourseLink.Url}#{id}")
                                );
                                break;
                            case 2 when sequence == null:
                                var anchorId = CommonVars.SelectedSection.Header.AnchorId;
                                HandleLink($"{CommonVars.SelectedCourseLink.Url}#{anchorId}");
                                break;
                            case 2 when sequence != null:
                                HandleMultipleLinks(CommonVars.SelectedSection.Links
                                    .Where((_, i) => sequence.Contains(i))
                                    .Select(link => link.Url)
                                );
                                break;
                            case 3:
                                HandleMultipleLinks(CommonVars.DownloadQueue.Select(l => l.Url).ToList());
                                break;
                        }
                    });
            }


            return this;
        }

        protected virtual void HandleLink(string link)
        {
            throw new System.NotImplementedException();
        }

        protected virtual void HandleMultipleLinks(IEnumerable<string> links)
        {
            throw new System.NotImplementedException();
        }
    }
}