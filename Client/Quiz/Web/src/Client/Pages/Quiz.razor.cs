using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwiftCaps.Client.Shared.Models;
using SwiftCaps.Models.Models;
using SwiftCAPS.Web.Shared.Clients;
using SwiftCAPS.Web.Shared.State;

namespace SwiftCAPS.Web.Client.Pages
{
    [Authorize(Roles = nameof(RoleConstants.User))]
    public partial class Quiz
    {
        private string _spinnerLabel = "Loading...";
        private string _infoText;
        private int _carouselIndex = 1;
        private int _currentQuizSectionIndex;
        private UserQuiz _userQuiz;
        private QuizSection _currentQuizSection;
        private bool IsDisabled => _currentQuizSection.Questions.Any(x => x.QuizAnswers.Any(a => !a.IsValid));
        private string _dialogMessage;
        private bool _isDialogOpen;
        private string _dialogTitle;
        [Parameter] public string Param { get; set; }
        [Inject] private IJSRuntime JsRuntime { get; set; }
        [Inject] private IQuizClient QuizClient { get; set; }
        [Inject] public UserState UserState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        public bool ShowLeftButton => (_currentQuizSection.Questions.Count > 0 && (_carouselIndex - 1) > 0);
        public bool ShowRightButton => (_currentQuizSection.Questions.Count > 0 && (_carouselIndex + 1) <= _currentQuizSection.Questions.Count);

        protected override void OnInitialized()
        {
            _currentQuizSectionIndex = int.Parse(Param ?? "0");
            if (UserState.UserQuiz is null)
            {
                _dialogTitle = "Error";
                _dialogMessage = "The quiz could not be loaded. Please try again.";
                _isDialogOpen = true;
            }
            else
            {
                _userQuiz = UserState.UserQuiz;
                _currentQuizSection = _userQuiz.Schedule.Quiz.QuizSections.Count > 0 ? _userQuiz.Schedule.Quiz.QuizSections[_currentQuizSectionIndex]: new QuizSection();
                if (_currentQuizSection.Questions == null)
                    _currentQuizSection.Questions = new List<Question>();
                _infoText = Markdown.ToHtml(_userQuiz.Schedule.Quiz.InfoMarkdown != null ? _userQuiz.Schedule.Quiz.InfoMarkdown : string.Empty);
            }
        }

        private async Task OnClickHandler()
        {
            _currentQuizSection.IsValid = true;

            if (_userQuiz.Schedule.Quiz.QuizSections.All(s => s.IsValid))
            {
                var quizName = _userQuiz.Schedule?.Quiz.Name;
                _spinnerLabel = "saving...";
                _userQuiz.Schedule = null;
                _userQuiz.Completed = DateTime.UtcNow;

                var response = await QuizClient.SaveUserQuiz(_userQuiz);

                if (response.IsOK())
                {
                    _dialogTitle = $"Congratulations {UserState.User.FirstName}";
                    _dialogMessage = $"You successfully completed the {quizName} quiz";
                }
                else
                {
                    _dialogTitle = "Error";
                    _dialogMessage = "There was a problem wrong saving the quiz. Please try again.";
                }
                _isDialogOpen = true;
            }
            else
            {
                _currentQuizSectionIndex++;
                _currentQuizSection = _userQuiz.Schedule.Quiz.QuizSections[_currentQuizSectionIndex];

                NavigationManager.NavigateTo($"/refresh/quiz/section/{_currentQuizSectionIndex}");
            }
        }

        private ValueTask OnPaging(int id)
        {
            if (id < 1 || id > _currentQuizSection.Questions.Count) return new ValueTask();

            _carouselIndex = id;
            return JsRuntime.InvokeVoidAsync("JsFunctions.carousel.scrollInToView", $"carouselItem{id}");
        }

        private string GetPagingStateClass(int index)
        {
            return _carouselIndex == index ? "is-active" : null;
        }

        private void OnAnswerChanged(int activeIndex)
        {
            OnPaging(activeIndex);
            StateHasChanged();
        }

        private void OnDismiss()
        {
            _isDialogOpen = false;
            _dialogTitle = string.Empty;
            _dialogMessage = string.Empty;
            NavigationManager.NavigateTo("/");
        }
    }
}
