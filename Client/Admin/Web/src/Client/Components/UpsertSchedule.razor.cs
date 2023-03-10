using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class UpsertSchedule
    {
        [Parameter] public EventCallback OnNavigationBackCallback { get; set; }
        [Parameter] public EventCallback OnCloseCallback { get; set; }
        [Inject] public IAdminQuizClient QuizClient { get; set; }
        [Inject] public IScheduleClient ScheduleClient { get; set; }

        private CommandItem _mode;

        private Guid? _scheduleId;

        private string _scheduleSaveError;

        private bool _isPanelOpen = false;

        private bool _showBackButton = false;
        private string _successMessageType => (_mode == CommandItem.Add ? "added" : "updated");

        private bool _isBusy = false;

        private bool _showErrorMessage => !string.IsNullOrEmpty(_scheduleSaveError);

        private bool _showSuccessMessage;

        private ScheduleViewModel model = new ScheduleViewModel();

        private EditContext editContext;

        private List<IDropdownOption>? _quizOptions;

        private IDropdownOption? _selectedQuiz { get; set; }

        private List<DropdownOption>? _recurrenceOptions = new List<DropdownOption>
        {
            new DropdownOption{ Key = ((int)Recurrence.Monthly).ToString(), Text= Recurrence.Monthly.ToString() },
            new DropdownOption{ Key = ((int)Recurrence.Weekly).ToString(), Text=Recurrence.Weekly.ToString() },
        };

        private IDropdownOption? _selectedRecurrence { get; set; }

        private async Task PanelDismissHandler()
        {
            _isPanelOpen = false;
            await OnCloseCallback.InvokeAsync();
        }

        private async Task OnStartDateChangeHandler(DateTime? updatedDate)
        {
            if (updatedDate.HasValue && model.EndTime < updatedDate)
            {
                model.EndTime = updatedDate;
            }
        }

        private async Task<ScheduleViewModel> GetSchedule()
        {
            ScheduleViewModel scheduleViewModel = null;
            try
            {
                if (_scheduleId.HasValue && _scheduleId != default)
                {
                    _isBusy = true;
                    var response = await ScheduleClient.GetSchedule(_scheduleId.Value);
                    if (response.IsOK())
                    {
                        var data = response.Data;
                        scheduleViewModel = new ScheduleViewModel
                        {
                            QuizId = data.QuizId,
                            Recurrence = data.Recurrence,
                            StartTime = data.StartTime?.ToLocalTime(),
                            EndTime = data.EndTime?.ToLocalTime(),
                        };

                        editContext = new(model);
                    }
                    else
                    {
                        _scheduleSaveError = "Error retrieving Schedule. Please try again.";
                    }
                }
            }
            catch
            {
                _scheduleSaveError = "Error retrieving Schedule. Please try again.";
            }
            finally
            {
                _isBusy = false;
            }
            return scheduleViewModel;
        }

        private async Task HandleValidSubmit()
        {
            if (editContext.Validate() && model.EndTime >= model.StartTime)
            {
                try
                {
                    _isBusy = true;
                    _scheduleSaveError = string.Empty;

                    var payload = new Schedule
                    {
                        QuizId = Guid.Parse(_selectedQuiz.Key),
                        Recurrence = Enum.Parse<Recurrence>(_selectedRecurrence.Key),
                        StartTime = model.StartTime,
                        EndTime = model.EndTime
                    };

                    ServiceResponse<Guid?> result;
                    if (_mode == CommandItem.Add)
                    {
                        result = await ScheduleClient.AddSchedule(payload);
                    }
                    else
                    {
                        payload.Id = _scheduleId.Value;
                        result = await ScheduleClient.UpdateSchedule(_scheduleId.Value, payload);
                    }

                    if (result.StatusCode != (int)HttpStatusCode.Accepted && result.StatusCode != (int)HttpStatusCode.Created)
                    {
                        _scheduleSaveError = "Error saving Schedule. Please try again.";
                        return;
                    }

                    _showSuccessMessage = true;
                }
                catch (Exception ex)
                {
                    _scheduleSaveError = "Error saving Schedule. Please try again.";
                }
                finally
                {
                    _isBusy = false;
                }
            }
        }

        async Task OnNavigationBackClick()
        {
            _isPanelOpen = false;
            await OnNavigationBackCallback.InvokeAsync();
        }

        private bool _isScheduleActive;
        private bool _isScheduleInPast;

        private DateTime GetMinEnddate()
        {
            var today = DateTime.Now.Date;
            var startDate = model.StartTime.HasValue ? model.StartTime.Value.Date : DateTime.Now.Date;

            if (startDate >= today)
            {
                return startDate;
            }
            else
            {
                return today;
            }
        }

        public async Task Show(Guid? scheduleId = null, bool showNavigation = false)
        {
            model = new ScheduleViewModel();
            _showBackButton = showNavigation;
            _isPanelOpen = true;
            _isScheduleActive = false;
            _mode = (scheduleId.HasValue && scheduleId.Value != default) ? CommandItem.Edit : CommandItem.Add;
            if (scheduleId.HasValue && scheduleId.Value != default)
            {
                _scheduleId = scheduleId;
                model = await GetSchedule();

                //check if schedule is active
                var today = DateTime.Now.Date;
                _isScheduleActive = model.StartTime?.Date <= today && model.EndTime?.Date >= today;

                //check if schedule has been passed
                _isScheduleInPast = model.EndTime?.Date < today && model.StartTime?.Date < today;
                if (_isScheduleInPast)
                {
                    _isScheduleActive = _isScheduleInPast;
                }
            }
            editContext = new EditContext(model);

            _showSuccessMessage = false;
            _scheduleSaveError = string.Empty;

            _quizOptions = await GetQuizzes();

            if (_mode == CommandItem.Add)
            {
                _selectedQuiz = _quizOptions.FirstOrDefault();

                _selectedRecurrence = _recurrenceOptions.FirstOrDefault();
            }
            else
            {
                _selectedQuiz = _quizOptions.Where(m => m.Key == model.QuizId.ToString()).FirstOrDefault();
                _selectedRecurrence = _recurrenceOptions.Where(m => m.Key == ((int)model.Recurrence).ToString()).FirstOrDefault();
            }

            StateHasChanged();
        }

        private async Task<List<IDropdownOption>> GetQuizzes()
        {
            var quizzes = new List<IDropdownOption>();
            var response = await QuizClient.GetQuizzes();
            if (response.IsOK())
            {
                quizzes = response.Data.Select(x => new DropdownOption
                {
                    Key = x.Id.ToString(),
                    Text = x.Title
                }).Cast<IDropdownOption>().ToList();
            }
            return quizzes;
        }
    }
}
