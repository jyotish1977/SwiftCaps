using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;

namespace SwiftCAPS.Web.Client.Components
{
    public partial class QuizCard
    {
        [Parameter] public EventCallback<UserQuiz> OnClick { get; set; }

        private UserQuiz _userQuiz;
        [Parameter]
        public UserQuiz UserQuiz
        {
            get => _userQuiz;
            set
            {
                _userQuiz = value;
                OnUserQuizChange(value);
            }
        }

        private TextType titleTextType = TextType.XLarge;
        private TextType subtitleTextType = TextType.SmallPlus;
        private TextType descriptionTextType = TextType.SmallPlus;
        private string _buttonClasses = "quizCardButton ";
        private string _titleClasses = "quizCardTitle ";
        private bool _isQuizCompleted;

        private string _title;
        private string _sectionsQuestionsCount;
        private string _buttonText;

        private void OnUserQuizChange(UserQuiz userQuiz)
        {
            if (userQuiz?.Schedule == null) return;

            if (userQuiz.Completed.HasValue)
            {
                // Quiz is completed
                _buttonText = "Complete";
                _title = "Completed";
                _buttonClasses += "quizCardCompletedButton ";
                _titleClasses += "quizCardCompletedTitle ";
                _isQuizCompleted = true;
            }
            else
            {
                var title = string.Empty;
                _buttonText = "Start Quiz";
                // Load the General Styles

                if (userQuiz.Schedule.Recurrence == Recurrence.Weekly)
                {
                    title = "WEEK {0} QUIZ";
                    _buttonClasses += "quizCardWeeklyButton ";
                    _titleClasses += "quizCardWeeklyTitle ";
                }
                else if (userQuiz.Schedule.Recurrence == Recurrence.Monthly)
                {
                    title = "MONTH {0} QUIZ";
                    _buttonClasses += "quizCardMonthlyButton ";
                    _titleClasses += "quizCardMonthlyTitle ";
                }

                _title = string.Format(title, userQuiz.Sequence);
                //_title = title;
            }


            var sectionCount = 0;
            var questionCount = 0;

            foreach (var quizSection in userQuiz.Schedule.Quiz.QuizSections)
            {
                sectionCount++;
                questionCount += quizSection.Questions.Count;
            }

            var sequence = userQuiz.Schedule.Recurrence == Recurrence.Weekly ? $"Week {userQuiz.Sequence}" : $"Month {userQuiz.Sequence}";

            _sectionsQuestionsCount = $" {sequence} / {sectionCount} Sections / {questionCount} Questions Total";
        }

        private async Task OnClickHandler()
        {
            await OnClick.InvokeAsync(UserQuiz);
        }
    }
}
