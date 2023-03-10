using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwiftCaps.Models.Models;

namespace SwiftCAPS.Web.Client.Components
{
    public partial class QuestionCard
    {
        private bool _isDialogOpen;

        private bool _isValid;

        [Parameter] public Question Question { get; set; }

        [Parameter] public EventCallback<int> AnswerChanged { get; set; }

        [Parameter] public string InfoText { get; set; }

        private Task OnAnswerChanged()
        {
            if (Question.QuizAnswers.All(a => a.IsValid) != _isValid)
            {
                _isValid = Question.QuizAnswers.All(a => a.IsValid);
            }

            AnswerChanged.InvokeAsync(Question.QuizSectionIndex);
            return Task.CompletedTask;
        }

        private void OnIconFocus(FocusEventArgs arg)
        {
            AnswerChanged.InvokeAsync(Question.QuizSectionIndex);
        }
    }
}
