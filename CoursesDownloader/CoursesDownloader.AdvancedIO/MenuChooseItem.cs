using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.AdvancedIO.SpecialActions;
using CoursesDownloader.AdvancedIO.SpecialActions.ConsoleActions;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.SharedVariables;

namespace CoursesDownloader.AdvancedIO
{
    public static class MenuChooseItem
    {
        public static T AskInputForSingleItemFromList<T>(IList<T> itemsList, string itemWord, RunningActionType? runningAction = null,
            string actionWord = "view", bool withClear = true, bool insideCall = false, bool breadcrumbs = true)
        {
            if (withClear)
            {
                ConsoleUtils.Clear();
            }

            if (!insideCall && breadcrumbs)
            {
                foreach (var prevItem in SharedVars.ChosenItemsTillNow.Values)
                {
                    ConsoleUtils.WriteLine(prevItem, ConsoleIOType.BreadCrumbs);
                }
            }

            ConsoleUtils.WriteLine($"Select the {itemWord} you want to {actionWord}:", ConsoleIOType.Question);

            var width = itemsList.Count.ToString().Length;
            for (var i = 0; i < itemsList.Count; i++)
            {
                var item = itemsList[i];
                var numbering = $"{i + 1}".PadLeft(width);
                if (item.ToString().Contains("(Recommended)"))
                {
                    ConsoleUtils.WriteLine($"[{numbering}] {item.ToString().FirstLine()}", ConsoleIOType.RecommendedOption);
                    var withoutFirstLine = item.ToString().WithoutFirstLine();
                    if (withoutFirstLine.IsNotEmpty())
                    {
                        ConsoleUtils.WriteLine(withoutFirstLine, ConsoleIOType.Options);
                    }
                }
                else
                {
                    ConsoleUtils.WriteLine($"[{numbering}] {item}", ConsoleIOType.Options);
                }
                
            }

            var selectedItem = default(T);

            var selectedItemIsValid = false;
            while (!selectedItemIsValid)
            {
                var chosenItem = ConsoleUtils.ReadLine($"And the {itemWord} that you selected is >>> ", ConsoleIOType.Question);

                if (chosenItem == null)
                {
                    throw new ExitAction();
                }

                var action = HandleAction(chosenItem);
                if (action.State == ActionState.FoundAndHandled)
                {
                    throw action;
                }

                if (!int.TryParse(chosenItem, out var itemIdx))
                {
                    continue;
                }

                itemIdx--;

                if (0 <= itemIdx && itemIdx < itemsList.Count)
                {
                    var confirmMessage = $"You chose: {itemsList[itemIdx].ToString().FirstLine().TrimInnerSpaces()} right? [Y/N] ";

                    AskYesNoQuestion(confirmMessage,
                        () =>
                        {
                            selectedItem = itemsList[itemIdx];
                            selectedItemIsValid = true;
                        });
                }
                else
                {
                    ConsoleUtils.WriteLine($"The {itemWord} you chose is out of bounds! Try again.", ConsoleIOType.Error);
                }
            }

            if (!insideCall && breadcrumbs)
            {
                SharedVars.ChosenItemsTillNow[runningAction.GetValueOrDefault()] = $"You chose {selectedItem?.ToString().FirstLine().TrimInnerSpaces()}";
            }

            return selectedItem;
        }

        public static IEnumerable<T> AskInputForMultipleItemsFromList<T>(IList<T> itemsList, string itemsWord, RunningActionType? runningAction = null,
            string actionWord = "download")
        {
            ConsoleUtils.Clear();

            foreach (var prevItem in SharedVars.ChosenItemsTillNow.Values)
            {
                ConsoleUtils.WriteLine(prevItem, ConsoleIOType.BreadCrumbs);
            }

            ConsoleUtils.WriteLine($"Select the {itemsWord} you want to {actionWord}: (range:x-y or set:x,y,z)", ConsoleIOType.Question);

            var width = itemsList.Count.ToString().Length;
            for (var i = 0; i < itemsList.Count; i++)
            {
                var item = itemsList[i];
                var numbering = $"{i + 1}".PadLeft(width);
                if (item.ToString().Contains("(Recommended)"))
                {
                    ConsoleUtils.WriteLine($"[{numbering}] {item.ToString().FirstLine()}", ConsoleIOType.RecommendedOption);
                    var withoutFirstLine = item.ToString().WithoutFirstLine();
                    if (withoutFirstLine.IsNotEmpty())
                    {
                        ConsoleUtils.WriteLine(withoutFirstLine, ConsoleIOType.Options);
                    }
                }
                else
                {
                    ConsoleUtils.WriteLine($"[{numbering}] {item}", ConsoleIOType.Options);
                }
            }

            var selectedItems = new List<int>();

            var selectedItemIsValid = false;
            while (!selectedItemIsValid)
            {
                var itemsIdx = ConsoleUtils.ReadLine($"And the {itemsWord} that you selected are >>> ", ConsoleIOType.Question);

                if (itemsIdx == null)
                {
                    throw new ExitAction();
                }

                var action = HandleAction(itemsIdx);
                if (action.State == ActionState.FoundAndHandled)
                {
                    throw action;
                }

                var result = ValidateInput(itemsIdx)?.ToList();

                if (result.IsEmptyOrNull())
                {
                    continue;
                }

                result?.AddRange(selectedItems);
                result = result.SortedUnique().ToList();

                if (result.Any())
                {
                    var fitResult = result.Where(x => x <= itemsList.Count).SortedUnique().ToList();

                    var valid = fitResult.SortedUnique().ToList();
                    var validString = BuildRangesString(valid);

                    if (!result.SequenceEqual(fitResult))
                    {
                        var invalid = result.Except(fitResult).SortedUnique().ToList();
                        var invalidString = BuildRangesString(invalid);

                        string[] choicesPossible;

                        if (validString.IsNotEmpty())
                        {
                            ConsoleUtils.WriteLine($"The following {itemsWord} are out of range: {invalidString}", ConsoleColor.Yellow);
                            ConsoleUtils.WriteLine($"However these {itemsWord} are in range: {validString}", ConsoleColor.Yellow);
                            choicesPossible = new[] {
                                $"Download only {validString} and ignore {invalidString}",
                                $"Download only {validString} and change {invalidString}",
                                $"Don't download {validString} and change the whole selection"
                            };
                        }
                        else
                        {
                            ConsoleUtils.WriteLine($"All of the following {itemsWord} are out of range: {invalidString}", ConsoleIOType.Error);
                            continue;
                        }

                        var choiceSelected = AskInputForSingleItemFromList(
                            choicesPossible, "choice", null, "choose", withClear: false, insideCall: true
                            );

                        if (choiceSelected == choicesPossible[0])
                        {
                            selectedItems.AddRange(valid);
                            selectedItemIsValid = true;
                        }
                        else if (choiceSelected == choicesPossible[1])
                        {
                            selectedItems.AddRange(valid);
                        }
                        else if (choiceSelected == choicesPossible[2])
                        {
                            selectedItems.Clear();
                        }
                    }
                    else
                    {
                        ConsoleUtils.WriteLine($"These {itemsWord} will be downloaded: {validString}",
                            ConsoleColor.Green);

                        AskYesNoQuestion("Is that your choice? [Y/N] ",
                            () =>
                            {
                                selectedItems.AddRange(valid);
                                selectedItemIsValid = true;
                            }, () =>
                            {
                                selectedItems.Clear();
                            });
                    }
                }
            }

            selectedItems = selectedItems.SortedUnique().ToList();

            SharedVars.ChosenItemsTillNow[runningAction.GetValueOrDefault()] = $"You chose {BuildRangesString(selectedItems)}";

            return selectedItems.Select(i => itemsList[i - 1]).ToList();
        }

        public static bool AskYesNoQuestion(string confirmMessage, Action onYes = null, Action onNo = null,
            Action<string> onOther = null)
        {
            var finalAnswer = false;

            var confirmationIsValid = false;
            while (!confirmationIsValid)
            {
                var confirmation = ConsoleUtils.ReadLine(confirmMessage, ConsoleIOType.YesNoQuestion) 
                                   ?? "no";
                var fullInputString = confirmation;
                confirmation = confirmation.IsNotEmpty() ? confirmation[0].ToString().ToLower() : "";

                if (confirmation == "y")
                {
                    finalAnswer = true;
                    onYes?.Invoke();
                    confirmationIsValid = true;
                }
                else if(confirmation != "n")
                {
                    if (onOther != null)
                    {
                        onOther.Invoke(fullInputString);
                        return true;
                    }
                    else
                    {
                        ConsoleUtils.WriteLine("Wrong answer! Try again.", ConsoleIOType.Error);
                    }
                }
                else
                {
                    onNo?.Invoke();
                    confirmationIsValid = true;
                }
            }

            return finalAnswer;
        }

        private static BaseAction HandleAction(string inputString)
        {
            var action = new BaseAction();
            action = action.Handle(inputString);
            return action;
        }
        
        #region Private Funcs

        internal static IEnumerable<int> ValidateInput(string inputString)
        {
            var validRegex = @"^\d+((,\d+)*|(\-\d+)*)+(\d+)?$";
            inputString = inputString.TrimInnerSpaces();

            if (Regex.IsMatch(inputString, validRegex))
            {
                var expanded = new List<int>();
                var sequences = inputString.Split(',');
                foreach (var sequence in sequences)
                {
                    var strLimits = sequence.Split('-');
                    if (strLimits.Any())
                    {
                        var limits = strLimits.Select(int.Parse).SortedUnique().ToList();

                        var lowerLimit = limits.First();
                        var upperLimit = limits.Last();

                        if (lowerLimit == upperLimit)
                        {
                            expanded.Add(lowerLimit);
                        }
                        else
                        {
                            expanded.AddRange(Enumerable.Range(lowerLimit, upperLimit - lowerLimit + 1));
                        }
                    }
                    else
                    {
                        expanded.Add(int.Parse(sequence));
                    }
                }

                return expanded.Where(i => i > 0).SortedUnique();
            }

            return null;
        }

        private static IEnumerable<int[]> FindRanges(IEnumerable<int> enumerable)
        {
            foreach (var group in enumerable.ConsecutiveGroupBy())
            {
                var groupList = group.ToList();
                if (groupList.Count == 1)
                {
                    yield return new[] { groupList[0] };
                }
                else
                {
                    yield return new[] { groupList[0], groupList.Last() };
                }
            }
        }

        private static string BuildRangesString(IEnumerable<int> enumerable)
        {
            return FindRanges(enumerable)
                .Select(docRange =>
                    docRange.Length == 0 ?
                        docRange[0].ToString() :
                        docRange.Join("-")
                        )
                .Join(", ");
        }

        #endregion
    }
}
