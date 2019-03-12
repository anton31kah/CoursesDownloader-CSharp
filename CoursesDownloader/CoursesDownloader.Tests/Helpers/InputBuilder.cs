using System.Linq;
using System.Text;

namespace CoursesDownloader.Tests.Helpers
{
    /// <example>
    /// <code>
    /// var input = new InputBuilder()
    ///     .WithAction(SwitchSemester)
    ///     .WithSingleOption(5) // switch to semester 5
    ///     .WithSingleOption(7) // open mpip
    ///     .WithAction(Add, "4,14", Yes, No) // add exercises 1 and labs (external links, pages, files)
    ///     .WithAction(Home) // go home
    ///     .WithSingleOption(5) // open mis
    ///     .WithSingleOption(5) // open course materials
    ///     .WithAction(Add, "2", Yes, No) // add homework assignments (folder)
    ///     .WithAction(Download) // start download
    ///     .WithSingleOption(2) // select courses naming
    ///     .GetResult();
    /// </code>
    /// </example>
    public class InputBuilder
    {
        public enum ActionWord
        {
            Back, Home, Close, Exit, Quit, Refresh,
            Add, Remove, Download, Queue,
            Copy, Open,
            SwitchSemester, LogOut, TempUserLogIn, TempUserLogOut
        }

        public enum Confirmation
        {
            Yes, No
        }

        private static string FirstLetterLower(Confirmation confirmation)
        {
            var firstChar = $"{confirmation}"[0];
            firstChar = char.ToLower(firstChar);
            return $"{firstChar}";
        }

        private readonly StringBuilder _input;

        public InputBuilder()
        {
            _input = new StringBuilder();
        }

        public InputBuilder WithSingleOption(int choice, Confirmation confirmation = Confirmation.Yes)
        {
            _input.AppendLine(choice.ToString());
            _input.AppendLine(FirstLetterLower(confirmation));

            return this;
        }

        public InputBuilder WithMultipleOptions(string choicePattern, Confirmation confirmation = Confirmation.Yes)
        {
            _input.AppendLine(choicePattern);
            _input.AppendLine(FirstLetterLower(confirmation));

            return this;
        }

        /// <param name="confirmations">will default to <c>new[] {Confirmation.Yes}</c> if left empty</param>
        public InputBuilder WithAction(ActionWord actionWord, string actionArgs = "", params Confirmation[] confirmations)
        {
            if (confirmations.Length == 0)
            {
                confirmations = new[] {Confirmation.Yes};
            }

            _input.AppendLine($"{actionWord} {actionArgs}");
            _input.AppendJoin("\n", confirmations.Select(FirstLetterLower));
            _input.AppendLine();

            return this;
        }

        public InputBuilder WithConfirmation(Confirmation confirmation)
        {
            _input.AppendLine(FirstLetterLower(confirmation));

            return this;
        }

        public string GetResult()
        {
            return _input.ToString();
        }
    }
}