using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using SwiftCAPS.Admin.Web.Shared.States;

namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class QuizDetail
    {
        [Inject] public IAdminQuizSectionClient QuizSectionClient { get; set; }

        [Inject] public UserState UserState { get; set; }

        [Inject] public IQuestionClient QuestionClient { get; set; }

        [Parameter]
        public Quiz Quiz { get; set; }

        [Parameter]
        public QuizSummaryItem QuizSummary { get; set; }

        [Parameter]
        public EventCallback OnEditCallback { get; set; }

        [Parameter]
        public EventCallback OnClosePanelCallback { get; set; }

        [Parameter]
        public EventCallback<PanelType> OnChangePanelTypeCallback { get; set; }

        [Parameter]
        public EventCallback OnDeleteInvoked { get; set; }

        public QuizSection SelectedQuizSection { get; set; }

        private Guid? _sectionId;

        private Guid? _questionId;

        private string _quizSaveError;
        private bool _showErrorMessage => !string.IsNullOrEmpty(_quizSaveError);

        public enum QuizDetailCommand
        {
            ViewDetail = 0,
            AddSection = 1,
            EditSection = 2,
            DeleteSection = 3,
            AddQuestion = 4,
            EditQuestion = 5,
            DeleteQuestion = 6
        }

        public enum SortOrder
        {
            Up = 1,
            Down = 2
        }

        public QuizDetailCommand _quizDetailCommand = QuizDetailCommand.ViewDetail;

        protected override async Task OnInitializedAsync()
        {
            UserState.OnChange += StateHasChanged;
            if (Quiz.QuizSections != null && Quiz.QuizSections.Count() > 0)
            {
                SelectedQuizSection = Quiz.QuizSections.First(x => x.Index == UserState.ActiveQuizDetailSection);
            }
        }

        public void Dispose()
        {
            UserState.OnChange -= StateHasChanged;
        }

        private void OnSectionClick(int index)
        {
            UserState.ActiveQuizDetailSection = index;
            SelectedQuizSection = Quiz.QuizSections.First(x => x.Index == index);
        }

        private async Task EditDetail()
        {
            await OnEditCallback.InvokeAsync();
        }
            
        private async Task UpdateSortOrder(SortOrder sortOrder, Question question)
        {
            var indexOrder = (sortOrder == SortOrder.Up) ? question.QuizSectionIndex - 1 : question.QuizSectionIndex + 1;

            var payload = new Question
            {
                Id = question.Id,
                QuizSectionId = question.QuizSectionId,
                Body = question.Body,
                Header = question.Header,
                Footer = question.Footer,
                QuizSectionIndex = indexOrder
            };
            
            var response = await QuestionClient.UpdateQuestion(question.Id, payload);
            if (response.StatusCode != (int)HttpStatusCode.Accepted && response.StatusCode != (int)HttpStatusCode.Created)
            {
                _quizSaveError = "Error saving Question. Please try again.";
                return;
            }

            var sections = await QuizSectionClient.GetQuizSection(question.QuizSectionId);
            if (sections.StatusCode == (int)HttpStatusCode.OK)
            {
                QuizSection quiz = sections.Data;
                SelectedQuizSection.Questions = quiz.Questions;
                StateHasChanged();
            }
        }

        [Inject] public IQuestionClient QuizQuestionClient { get; set; }
        
        private async Task DeleteQuiz()
        {
            await OnDeleteInvoked.InvokeAsync();
        }

        private async Task AddSection()
        {
            _sectionId = null;
            _quizDetailCommand = QuizDetailCommand.AddSection;
            await OnChangePanelTypeCallback.InvokeAsync(PanelType.Medium);
        }

        private async Task EditSection(Guid sectionId)
        {
            _sectionId = sectionId;
            _quizDetailCommand = QuizDetailCommand.EditSection;
            await OnChangePanelTypeCallback.InvokeAsync(PanelType.Medium);
        }

        private async Task DeleteSection(Guid sectionId)
        {
            _sectionId = sectionId;
            _quizDetailCommand = QuizDetailCommand.DeleteSection;
            await OnChangePanelTypeCallback.InvokeAsync(PanelType.SmallFixedFar);
        }

        private async Task AddQuestion()
        {
            _questionId = null;
            _quizDetailCommand = QuizDetailCommand.AddQuestion;
            await OnChangePanelTypeCallback.InvokeAsync(PanelType.Medium);
        }

        private async Task EditQuestion(Guid questionId)
        {
            _questionId = questionId;
            _quizDetailCommand = QuizDetailCommand.EditQuestion;
            await OnChangePanelTypeCallback.InvokeAsync(PanelType.Medium);
        }

        private async Task DeleteQuestion(Guid questionId)
        {
            _questionId = questionId;
            _quizDetailCommand = QuizDetailCommand.DeleteQuestion;
            await OnChangePanelTypeCallback.InvokeAsync(PanelType.SmallFixedFar);
        }

        private async Task DeletePanelCloseHandler()
        {
            await OnClosePanelCallback.InvokeAsync();
        }

        private async Task<bool> QuizSectionDeleteHandler()
        {
            try
            {
                if (_sectionId != null)
                {
                    var response = await QuizSectionClient.DeleteQuizSection(_sectionId.Value);
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        UserState.ActiveQuizDetailSection = 1;
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private async Task<bool> QuestionDeleteHandler()
        {
            try
            {
                if (_questionId != null)
                {
                    var response = await QuestionClient.DeleteQuestion(_questionId.Value);
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        UserState.ActiveQuizDetailSection = UserState.ActiveQuizDetailSection;
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private string GetMarkup(string markdown)
        {
            if (!string.IsNullOrEmpty(markdown))
                return Markdig.Markdown.ToHtml(markdown);
            else
                return markdown;
        }

        public void OnPivotClick(PivotItem pivotItem, MouseEventArgs mouseEventArgs)
        {
            UserState.ActiveQuizDetailTab = pivotItem.ItemKey;
        }
    }
}
