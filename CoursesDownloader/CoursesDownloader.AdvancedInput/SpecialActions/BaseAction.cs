using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.Common.ExtensionMethods;

namespace CoursesDownloader.AdvancedIO.SpecialActions
{
    public class BaseAction : Exception
    {
        protected virtual string Type => "Base";
        protected virtual string Description => "This is the Base action, it is the father of all the actions";
        public ActionState State { get; protected set; }
        private static readonly string[] Children = {
            "Back", "Home",
            "Open", "Copy",
            "Exit", "Quit", "Close",
            "Refresh",
            "Add", "Remove", "Queue", "Download",
            "TempUserLogIn", "TempUserLogOut", "SwitchSemester"
        };
        private static readonly string[] HelpKeywords = {"Help", "Actions", "???"};

        public virtual BaseAction Handle(string inputString)
        {
            State = ActionState.NotFound;

            // First check if normal action
            var words = Children.Join("|");
            var actionFound = Regex.Match(inputString, $@"\b(?:{words})\b", RegexOptions.IgnoreCase);

            if (actionFound.Success)
            {
                var actionObject = GetActionObjectFrom(actionFound.Value);
                
                // If is action? 
                if (actionObject.IsAskingForHelp(inputString))
                {
                    ConsoleUtils.WriteLine(actionObject.Description, ConsoleColor.Yellow);
                    return this;
                }

                actionObject.Handle(inputString);
                return actionObject;
            }

            // Then check if help action
            words = HelpKeywords.Select(Regex.Escape).Join("|");
            actionFound = Regex.Match(inputString, $@"(?:{words})", RegexOptions.IgnoreCase);

            if (actionFound.Success)
            {
                foreach (var actionName in Children)
                {
                    var actionObject = GetActionObjectFrom(actionName);
                    ConsoleUtils.WriteLine($"{actionName}: {actionObject.Description}", ConsoleColor.Yellow);
                }
            }
            
            return this;
        }

        private static BaseAction GetActionObjectFrom(string actionName)
        {
            var actionCorrectName = Children.Single(action => action.Equals(actionName, StringComparison.OrdinalIgnoreCase));
            
            var actionClassName = actionCorrectName + "Action";

            var actionClassFullName = Assembly.GetExecutingAssembly().GetTypes()
                .First(a => a.FullName.Contains(actionClassName)).FullName;
            var actionObject =
                (BaseAction) Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(actionClassFullName));
            return actionObject;
        }

        protected void ConfirmAction(string outcome, Action onIsYes)
        {
            var confirmMessage = string.Join("\n",
                $"I noticed that you entered {Type.ToLower()}, which will {outcome}",
                "Is that the action you wanted to perform (answer no if it was entered by mistake)? [Y/N] "
            );
            MenuChooseItem.AskYesNoQuestion(confirmMessage, onIsYes);
        }

        private bool IsAskingForHelp(string inputString)
        {
            return inputString.ToLower().Contains($"{Type}?".ToLower());
        }

        public virtual void SetNextRunningActionType()
        {
        }
    }
}
