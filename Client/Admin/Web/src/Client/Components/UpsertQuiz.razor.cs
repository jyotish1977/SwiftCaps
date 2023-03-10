using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;

namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class UpsertQuiz
    {
        [Parameter]
        public string Heading { get; set; }

        [Parameter]
        public CommandItem Mode { get; set; }

        [Parameter]
        public QuizSummaryItem QuizSummary { get; set; }

        [Inject] public IAdminQuizClient QuizClient { get; set; }

        private Guid? _quizId;

        private string _quizSaveError;

        private string _successMessageType => (Mode == CommandItem.Add ? "added" : "updated");

        private bool _isBusy = false;

        private Dictionary<string, object> InfoTextFieldAttributes = new Dictionary<string, object>
        {
            { "rows", "15" }
        };

        private bool _showErrorMessage => !string.IsNullOrEmpty(_quizSaveError);
        private bool _showSuccessMessage;

        private QuizViewModel model = new QuizViewModel();

        private EditContext editContext;

        [Parameter]
        public EventCallback OnItemInvoked { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Mode == CommandItem.Edit)
            {
                _quizId = QuizSummary?.Id;
                await GetQuiz();
            }
            else
            {
                editContext = new(model);
            }
        }

        private async Task GetQuiz()
        {
            try
            {
                if (_quizId.HasValue && _quizId != default)
                {
                    _isBusy = true;
                    var response = await QuizClient.GetQuiz(_quizId.Value);
                    if (response.StatusCode == (int)HttpStatusCode.OK)
                    {
                        var data = response.Data;
                        model = new QuizViewModel
                        {
                            Name = data.Name,
                            Description = data.Description,
                            InfoMarkdown = data.InfoMarkdown
                        };
                        editContext = new(model);
                    }
                    else
                    {
                        _quizSaveError = "Error retrieving Quiz. Please try again.";
                    }
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
        }

        private async Task HandleValidSubmit()
        {
            if (editContext.Validate())
            {
                try
                {
                    _isBusy = true;
                    _quizSaveError = string.Empty;

                    if (Mode == CommandItem.Add)
                    {
                        var response = await QuizClient.AddQuiz(new Quiz
                        {
                            Name = model.Name,
                            Description = model.Description,
                            InfoMarkdown = model.InfoMarkdown
                        });
                        if (response.StatusCode != (int)HttpStatusCode.Created)
                        {
                            _quizSaveError = "Error saving Quiz. Please try again.";
                            return;
                        }
                    }
                    else
                    {
                        var response = await QuizClient.UpdateQuiz(_quizId.Value, new Quiz
                        {
                            Id = _quizId.Value,
                            Name = model.Name,
                            Description = model.Description,
                            InfoMarkdown = model.InfoMarkdown
                        });
                        if (response.StatusCode != (int)HttpStatusCode.Accepted)
                        {
                            _quizSaveError = "Error saving Quiz. Please try again.";
                            return;
                        }
                    }

                    _showSuccessMessage = true;
                    await OnItemInvoked.InvokeAsync();
                }
                catch (Exception ex)
                {
                    _quizSaveError = "Error saving Quiz. Please try again.";
                }
                finally
                {
                    _isBusy = false;
                }
            }
        }
    }
}
