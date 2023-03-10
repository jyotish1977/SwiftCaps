using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SwiftCAPS.Web.Client.Components
{
    public partial class QuizAnswer
    {
        [Parameter] public int QuestionIndex { get; set; }

        [Parameter] public SwiftCaps.Models.Models.QuizAnswer Answer { get; set; }

        [Parameter] public EventCallback<int> AnswerChanged { get; set; }

        private Task OnAnswerChanged(string value)
        {
            Answer.UserAnswer = value;
            return AnswerChanged.InvokeAsync(QuestionIndex);
        }

        private Task OnFocusChanged(FocusEventArgs args)
        {
            return AnswerChanged.InvokeAsync(QuestionIndex);
        }
    }
}
