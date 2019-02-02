using System.Collections.Generic;
using System.Linq;
using CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions.QueueModificationBaseActionHelpers;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO.SpecialActions.DownloadsActions
{
    public class QueueModificationBaseAction : BaseAction
    {
        protected override string Type => "QueueModification";
        protected override string Description => $"{Type}s selection (or last chosen selection) (recursively) {ToFromWord.ToLower()} Download Queue";

        public IEnumerable<int> MatchingItems { get; private set; }
        public ItemTypeToAddRemove MatchingItemType { get; private set; }
        protected virtual string ToFromWord => "To/From";

        /// <summary>
        /// if no numbers given => add/remove previously selected links or whole course or section
        /// if numbers given    => add/remove selected links or whole course or section
        /// </summary>
        public override BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            if (inputString.ToLower().Contains(Type.ToLower()))
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

                ConfirmAction(
                    $"will {Type.ToLower()} selected/chosen items (recursively) {ToFromWord.ToLower()} download queue",
                    () =>
                    {
                        State = ActionState.FoundAndHandled;
                        AddRemoveSelectedItems(sequence);
                    });
            }

            return this;
        }

        private void AddRemoveSelectedItems(IEnumerable<int> sequence)
        {
            var stagesDepth = SharedVars.ChosenItemsTillNow.Count;
            switch (stagesDepth)
            {
                case 0: // add/remove selected courses
                    MatchingItemType = ItemTypeToAddRemove.Course;
                    MatchingItems = sequence?.Where(i => i < SharedVars.Courses.Count) ??
                                    Enumerable.Range(0, SharedVars.Courses.Count);
                    break;
                case 1: // add/remove selected section
                    MatchingItemType = ItemTypeToAddRemove.Section;
                    MatchingItems = sequence?.Where(i => i < SharedVars.Sections.Count) ??
                                    Enumerable.Range(0, SharedVars.Sections.Count);
                    break;
                case 2: // add/remove selected link
                    MatchingItemType = ItemTypeToAddRemove.Link;
                    MatchingItems = sequence?.Where(i => i < SharedVars.SelectedSection.Count) ??
                                    Enumerable.Range(0, SharedVars.SelectedSection.Count);
                    break;
                case 4: // ignore
                    MatchingItemType = ItemTypeToAddRemove.Link;
                    MatchingItems = Enumerable.Empty<int>();
                    break;
            }
        }
    }
}