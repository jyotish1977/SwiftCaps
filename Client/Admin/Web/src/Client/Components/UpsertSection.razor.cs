using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class UpsertSection
    {
        [Inject] public IAdminQuizSectionClient QuizSectionClient { get; set; }

        [Parameter]
        public Guid? SectionId { get; set; }

        [Parameter]
        public Guid QuizId { get; set; }

        public CommandItem _mode;

        private string _successMessageType => (_mode == CommandItem.Add ? "added" : "updated");

        private EditContext editContext;

        private QuizSectionViewModel model = new QuizSectionViewModel();

        private bool _isBusy = false;

        private string _quizSaveError;
        private bool _showErrorMessage => !string.IsNullOrEmpty(_quizSaveError);
        private bool _showSuccessMessage;
        protected override async Task OnInitializedAsync()
        {
            if (SectionId.HasValue && SectionId.Value != default)
            {
                _mode = CommandItem.Edit;
                await GetQuizSection();
            }
            else
            {
                _mode = CommandItem.Add;
                editContext = new(model);
            }
        }

        private async Task GetQuizSection()
        {
            try
            {
                _isBusy = true;
                var response = await QuizSectionClient.GetQuizSection(SectionId.Value);
                if (response.StatusCode == (int)HttpStatusCode.OK)
                {
                    var data = response.Data;
                    model = new QuizSectionViewModel
                    {
                        Description = data.Description
                    };
                    editContext = new(model);
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
        }

        private async Task HandleValidSubmit()
        {
            if (editContext.Validate())
            {
                try
                {
                    _isBusy = true;
                    _quizSaveError = string.Empty;

                    var payload = new QuizSection
                    {
                        Description = model.Description,
                        QuizId = QuizId,
                    };

                    ServiceResponse<Guid?> result;
                    if (_mode == CommandItem.Add)
                    {
                        result = await QuizSectionClient.AddQuizSection(payload);
                    }
                    else
                    {
                        payload.Id = SectionId.Value;
                        result = await QuizSectionClient.UpdateQuizSection(SectionId.Value, payload);
                    }

                    if (result.StatusCode != (int)HttpStatusCode.Accepted && result.StatusCode != (int)HttpStatusCode.Created)
                    {
                        _quizSaveError = "Error saving Quiz section. Please try again.";
                        return;
                    }

                    _showSuccessMessage = true;
                }
                catch
                {
                    _quizSaveError = "Error saving Quiz section. Please try again.";
                }
                finally
                {
                    _isBusy = false;
                }
            }
        }
    }
}
