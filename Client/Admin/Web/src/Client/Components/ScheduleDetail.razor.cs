using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using SwiftCAPS.Blazor.Components;

namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class ScheduleDetail
    {
        [Inject] private IScheduleClient ScheduleClient { get; set; }

        [Parameter] public EventCallback OnCloseCallback { get; set; }

        private Schedule _schedule;

        private int _groupItemDisplayCount = 4;

        private bool _isBusy = false;

        private string _scheduleSaveError;

        private string _activeScheduleDetailTab = DetailsPivotType.General.ToString();
        private bool _showErrorMessage => !string.IsNullOrEmpty(_scheduleSaveError);

        private bool _canShowGrid => !_isBusy && string.IsNullOrEmpty(_scheduleSaveError) && _schedule != null;

        private bool _isPanelOpen;
        private UpsertSchedule upsertPanel;
        private DeletePanel deletePanel;

        private ScheduleGroups _scheduleGroups;
        async Task PanelDismissHandler()
        {
            _isPanelOpen = false;
            await OnCloseCallback.InvokeAsync();
        }

        private async Task OnScheduleReload()
        {
            await GetSchedule(_schedule?.Id);
        }

        private async Task<HttpResponseMessage> DeleteHandler()
        {
            return await ScheduleClient.DeleteSchedule(_schedule.Id);
        }

        public async Task Show(Guid? scheduleId = null)
        {
            try
            {
                if (scheduleId != null && scheduleId != default)
                {
                    _isBusy = true;
                    _isPanelOpen = true;
                    _activeScheduleDetailTab = DetailsPivotType.General.ToString();
                    await GetSchedule(scheduleId);
                }
            }
            catch
            {
                _scheduleSaveError = "Error loading Schedule. Please try again.";
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async Task GetSchedule(Guid? scheduleId)
        {
            _isBusy = true;
            var response = await ScheduleClient.GetSchedule(scheduleId.Value);
            if (!response.IsOK())
                throw new Exception(string.Join(',', response.Errors));

            _isBusy = false;
            _schedule = response.Data;
            StateHasChanged();
        }

        private async Task OnManageGroupsClick()
        {
            await _scheduleGroups.Show(_schedule?.Id, true);

        }

        private async Task EditDetail()
        {
            await upsertPanel.Show(_schedule.Id, true);
        }

        private async Task OnScheduleDelete()
        {
            await deletePanel.Show("Schedule", true);
        }

        private async Task UpsertNavigationBackHandler()
        {
            await Show(_schedule.Id);
        }

        private async Task UpsertCloseHandler()
        {
            _isPanelOpen = false;
            await OnCloseCallback.InvokeAsync();
        }

        private void OnPivotClick(PivotItem pivotItem, MouseEventArgs mouseEventArgs)
        {
            _activeScheduleDetailTab = pivotItem.ItemKey;
            StateHasChanged();
        }




        async Task NavigationBackHandler()
        {
            await Show(_schedule?.Id);
            await Task.CompletedTask;
        }
        async Task CloseHandler()
        {
            _isPanelOpen = false;
            await Task.CompletedTask;
        }
    }
}
