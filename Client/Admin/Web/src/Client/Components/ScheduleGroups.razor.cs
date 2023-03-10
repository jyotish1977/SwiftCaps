using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using SwiftCAPS.Admin.Web.Shared.States;
using SwiftCAPS.Blazor.Components;
using System.Net.Http;

namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class ScheduleGroups
    {
        [Inject] private IScheduleGroupClient ScheduleGroupClient { get; set; }
        
        [Parameter] public EventCallback OnNavigationBackCallback { get; set; }
        [Parameter] public EventCallback OnCloseCallback { get; set; }
       
        [Parameter] public EventCallback OnClosePanelCallback { get; set; }

        bool _isPanelOpen = false;
        bool _showBackButton = false;
        private Guid _scheduleId;

        private readonly List<DetailsRowColumn<ScheduleGroupSummary>> _columns = new();
        private List<ScheduleGroupSummary> SchedulesGroupList;
        private Selection<ScheduleGroupSummary> _selectedGroupSummary;

        private bool _isBusy = false;
        private string _groupListError;
        private bool _canShowGrid => !_isBusy && string.IsNullOrEmpty(_groupListError);
        private bool _showErrorMessage => !string.IsNullOrEmpty(_groupListError);
        
        
        private ICommand _buttonCommand;
        private List<CommandBarItem> _commandItems;
        private AddScheduleGroups _addScheduleGroups;
        private DeletePanel _deletePanel;

        protected override async Task OnInitializedAsync()
        {
            
                _isBusy = true;
                AddColumns();

                ConfigureCommandBar();
                _selectedGroupSummary = new Selection<ScheduleGroupSummary>();

                _selectedGroupSummary.SelectionChanged.Subscribe(m =>
                {
                    InvokeAsync(StateHasChanged);
                });
              _isBusy = false;

        }

        /// <summary>
        /// Refresh icon 
        /// </summary>
        /// <returns></returns>
        private async Task OnReloadGroups()
        {
            await LoadScheduleGroups();
        }
 
        async Task NavigationBackHandler()
        {
            await LoadScheduleGroups();
            await Task.CompletedTask;
        }
        async Task OnCloseClick()
        {
            _isPanelOpen = false;
            
            await OnCloseCallback.InvokeAsync();
        }

        async Task OnNavigationBackClick()
        {
            _isPanelOpen = false;
            await OnNavigationBackCallback.InvokeAsync();
        }

        public async Task Show(Guid? scheduleId, bool showNavigation = false)
        {
            if (scheduleId != null)
            {
                _scheduleId = (Guid)scheduleId;
                _showBackButton = showNavigation;
                 await LoadScheduleGroups();
                _isPanelOpen = true;
                StateHasChanged();
            }
        }

        async Task CloseHandler()
        {
            _isPanelOpen = false;
            await OnCloseCallback.InvokeAsync();
            
        }

       

      
      

        private async Task LoadScheduleGroups()
        {
            try
            {
                _isBusy = true;
                _groupListError = string.Empty;
                var response = await ScheduleGroupClient.GetSecheduleGroups(_scheduleId);
                if (!response.IsOK())
                {
                    _groupListError = "Error loading schedule groups. Please try again.";
                    SchedulesGroupList = new List<ScheduleGroupSummary>();
                }
                else         
                    SchedulesGroupList = response.Data.OrderBy(x => x.GroupName).ToList();
            }
            catch
            {
                _groupListError = "Error loading schedule groups. Please try again.";
            }
            finally
            {
                _isBusy = false;
            }
        }
 
        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<ScheduleGroupSummary>("Group ", x => x.GroupName) { MinWidth = 30, MaxWidth = 300, Index = 0, OnColumnClick = OnGroupColumnClick, IsResizable = true, });

            _columns.Add(new DetailsRowColumn<ScheduleGroupSummary>("Members ", x => x.UserCount) { MinWidth = 30, MaxWidth = 300, Index = 1, IsResizable = true, });

        }

        private void OnGroupColumnClick(IDetailsRowColumn<ScheduleGroupSummary> column)
        {
            if (column.IsSortedDescending && column.IsSorted)
                column.IsSorted = !column.IsSorted;

            if (column.IsSorted)
            {

                SchedulesGroupList = new List<ScheduleGroupSummary>(column.IsSorted ? SchedulesGroupList.OrderBy(x => x.GroupName) : SchedulesGroupList.OrderByDescending(x => x.GroupName));
                column.IsSortedDescending = !column.IsSortedDescending;
            }
            else
            {
                SchedulesGroupList = new List<ScheduleGroupSummary>(column.IsSorted ? SchedulesGroupList.OrderBy(x => x.GroupName) : SchedulesGroupList.OrderByDescending(x => x.GroupName));
                column.IsSorted = !column.IsSorted;
                column.IsSortedDescending = false;
            }
            StateHasChanged();
        }

        

        public void ConfigureCommandBar()
        {
            _buttonCommand = new RelayCommand((item) =>
            {
                if (item != null)
                {
                    CommandItemClick((string)item);
                    StateHasChanged();
                }
            });

            _commandItems = new List<CommandBarItem> {
                new CommandBarItem() {  Text= "Add Group", IconName="Add", Key=CommandItem.Add.ToString()},
                new CommandBarItem() {  Text= "Delete Group",Disabled=true, IconName="Delete", Key=CommandItem.Delete.ToString()}
            };
        }

        private async Task CommandItemClick(string item)
        {
            if (item == CommandItem.Add.ToString())
            {
                await _addScheduleGroups.Show(_scheduleId, true);
            }

            if (item == CommandItem.Delete.ToString())
            {
                string _caption = "Schedule Group";

                if(_selectedGroupSummary.SelectedItems.Count > 1)
                    _caption = "Schedule Groups";
                await _deletePanel.Show(_caption, true);
            }
        }

        private async Task<HttpResponseMessage> DeleteHandler()
        {
            dynamic response = null;
            try
            {
                _isBusy = true;
            
                foreach (var group in _selectedGroupSummary.SelectedItems)
                {
                     response = await ScheduleGroupClient.DeleteScheduleGroup(_scheduleId, group.GroupId);
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        return response;
                }

            }
            catch
            {

            }
            finally
            {
                _isBusy = true;
            }
            return response;
        }


        async Task PanelDismissHandler()
        {
            _isPanelOpen = false;
            await OnCloseCallback.InvokeAsync();
        }

    }
}
