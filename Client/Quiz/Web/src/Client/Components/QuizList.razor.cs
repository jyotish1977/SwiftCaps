using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Models;

namespace SwiftCAPS.Web.Client.Components
{
    public partial class QuizList
    {
        [Parameter] public ICollection<UserQuiz> UserQuizzes { get; set; }
        [Parameter] public string Placeholder { get; set; }
        [Parameter] public EventCallback<UserQuiz> OnQuizClick { get; set; }

        private async Task QuizClickHandler(UserQuiz quiz)
        {
            await OnQuizClick.InvokeAsync(quiz);
        }
    }
}
