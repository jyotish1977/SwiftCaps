using System.Linq;
using SwiftCaps.Client.Core.Enums;
using SwiftCaps.Models.Models;
using Xamarin.Forms;

namespace SwiftCaps.Triggers
{
    /// <summary>
    /// Trigger used for auto focusing from one Entry
    /// to the other Entry, upon correct answer typing
    /// in the QuestionCardTemplate
    /// </summary>
    public class AnswerValidTriggerAction : TriggerAction<Entry>
    {
        public QuizAnswersLayout QuizAnswersLayoutStyle { get; set; }

        protected override void Invoke(Entry sender)
        {
            if (QuizAnswersLayoutStyle == QuizAnswersLayout.InLine)
            {
                // Handling in INLINE Quiz Answer Style

                if (((Grid)sender.Parent).Parent == null)
                    return; // to avoid any empty reference errors

                var currentQuestion = ((FlexLayout)((Grid)sender.Parent).Parent).BindingContext as Question;

                // Check if there's any pending answers
                if (currentQuestion != null && currentQuestion.QuizAnswers.Any(x => !x.IsValid))
                {
                    // Then get the next pending answers
                    var pendingAnswer = currentQuestion.QuizAnswers.First(x => !x.IsValid);
                    var entryParentGridList =
                        ((FlexLayout)((Grid)sender.Parent).Parent).Children.Where(x => x.GetType() == typeof(Grid));
                    foreach (var grid in entryParentGridList)
                    {
                        // Traverse through all the Entry elements
                        var pendingAnswerEntry =
                            grid.LogicalChildren.FirstOrDefault(x => x.BindingContext == pendingAnswer);
                        if (pendingAnswerEntry != null)
                        {
                            // Found the next pending answer, so we focus on that Entry!
                            ((Entry)pendingAnswerEntry).Focus();
                            break;
                        }
                    }
                }
            }
            else
            {
                // Handling in SEPARATE Quiz Answer Style

                if ((StackLayout)sender.Parent == null)
                    return; // to avoid any empty reference errors

                var currentQuestion = ((StackLayout)sender.Parent).BindingContext as Question;

                // Check if there's any pending answers
                if (currentQuestion != null && currentQuestion.QuizAnswers.Any(x => !x.IsValid))
                {
                    // Then get the next pending answer
                    var pendingAnswer = currentQuestion.QuizAnswers.First(x => !x.IsValid);
                    var pendingAnswerEntry =
                        ((StackLayout)sender.Parent).Children.First(x => x.BindingContext == pendingAnswer);

                    // Found the next pending answer, so we focus on that Entry!
                    ((Entry)pendingAnswerEntry).Focus();
                }
            }
        }
    }
}
