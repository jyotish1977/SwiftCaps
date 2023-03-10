using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class UpsertQuestion
    {
        [Inject] public IQuestionClient QuizQuestionClient { get; set; }

        [Inject] public IAdminQuizSectionClient QuizSectionClient { get; set; }

        [Parameter]
        public QuizSection Section { get; set; }

        [Parameter]
        public Guid? QuestionId { get; set; }

        [Parameter]
        public Guid QuizId { get; set; }

        public CommandItem _mode;

        private Dictionary<string, object> InfoTextFieldAttributes = new Dictionary<string, object>
        {
            { "rows", "8" }
        };

        private string _successMessageType => (_mode == CommandItem.Add ? "added" : "updated");

        public IDropdownOption? _selectSection { get; set; }

        private EditContext editContext;

        private QuestionViewModel model = new QuestionViewModel();

        private bool _isBusy = false;

        List<IDropdownOption>? _sections;

        private string _quizSaveError;
        private bool _showErrorMessage => !string.IsNullOrEmpty(_quizSaveError);
        private bool _showSuccessMessage;
        protected override async Task OnInitializedAsync()
        {
            _mode = (QuestionId.HasValue && QuestionId.Value != default) ? CommandItem.Edit : CommandItem.Add;

            if (_mode == CommandItem.Edit)
            {
                model = await GetViewModel();
            }

            editContext = new(model);

            _sections = await GetSections();

            _selectSection = _sections.First(x => x.Key == Section.Id.ToString());
        }

        private async Task<List<IDropdownOption>> GetSections()
        {
            var sections = new List<IDropdownOption>();
            var response = await QuizSectionClient.GetQuizSections(Section.QuizId);
            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                sections = response.Data.Select(x => new DropdownOption
                {
                    Key = x.Id.ToString(),
                    Text = x.Description
                }).Cast<IDropdownOption>().ToList();
            }
            return sections;
        }

        private async Task<QuestionViewModel> GetViewModel()
        {
            QuestionViewModel viewModel = null;
            try
            {
                _isBusy = true;
                var response = await QuizQuestionClient.GetQuestion(QuestionId.Value);
                if (response.StatusCode == (int)HttpStatusCode.OK)
                {
                    var data = response.Data;
                    viewModel = new QuestionViewModel
                    {
                        QuizSectionIndex = data.QuizSectionIndex,
                        QuizSectionId = data.QuizSectionId,
                        Body = data.Body,
                        Header = data.Header,
                        Footer = data.Footer
                    };
                }
                else
                {
                    _quizSaveError = "Error retrieving Quiz. Please try again.";
                }
            }
            catch
            {
                _quizSaveError = "Error retrieving Quiz. Please try again.";
            }
            finally
            {
                _isBusy = false;
            }
            return viewModel;
        }

        private async Task HandleValidSubmit()
        {
            if (editContext.Validate())
            {
                try
                {
                    _isBusy = true;
                    _quizSaveError = string.Empty;

                    var payload = new Question
                    {
                        QuizSectionIndex = model.QuizSectionIndex,
                        Body = model.Body,
                        Header = model.Header,
                        Footer = model.Footer,
                        QuizSectionId = Guid.Parse(_selectSection.Key)
                    };

                    Task<ServiceResponse<Guid?>> result;
                    if (_mode == CommandItem.Add)
                    {
                        result = QuizQuestionClient.AddQuestion(payload);
                    }
                    else
                    {
                        payload.Id = QuestionId.Value;
                        result = QuizQuestionClient.UpdateQuestion(QuestionId.Value, payload);
                    }

                    var response = await result;

                    if (response.StatusCode != (int)HttpStatusCode.Accepted && response.StatusCode != (int)HttpStatusCode.Created)
                    {
                        _quizSaveError = "Error saving Question. Please try again.";
                        return;
                    }

                    _showSuccessMessage = true;
                }
                catch
                {
                    _quizSaveError = "Error saving Question. Please try again.";
                }
                finally
                {
                    _isBusy = false;
                }
            }
        }
    }
}
