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


namespace SwiftCAPS.Admin.Web.Client.Components
{
    public partial class AddScheduleGroups
    {
        [Inject] private IScheduleGroupClient ScheduleGroupClient { get; set; }
        [Parameter] public EventCallback OnNavigationBackCallback { get; set; }
        [Parameter] public EventCallback OnCloseCallback { get; set; }
        
        bool _isPanelOpen = false;
        bool showBackButton = true;
        private bool _displayPanelBackButton = true;
        string? SelectedItem { get; set; }
        
        private readonly List<DetailsRowColumn<Group>> _columns = new();
        private bool _isBusy = false;
        private string _addGroupsListError;
        private bool _canShowGrid => !_isBusy && string.IsNullOrEmpty(_addGroupsListError);
        private bool _showErrorMessage => !string.IsNullOrEmpty(_addGroupsListError);
        private Guid _sheduleId;
        private List<Group> _groups;
        private Selection<Group> _selectedGroup;
        private TextField _searchField;

         
        private async Task OnSearch()
        {
            await LoadScheduleGroups();


        }
            
        private async Task OnReloadScheduleGroups()
        {
            _searchField.Value = string.Empty;
            await LoadScheduleGroups();

        }
             
        protected override async Task OnInitializedAsync()
        {
            AddColumns();

            _selectedGroup = new Selection<Group>();
            _selectedGroup.SelectionChanged.Subscribe(m =>
            {
                InvokeAsync(StateHasChanged);
            });
        }
        
        async Task OnPanelDismiss()
        {
            _isPanelOpen = false;
            await OnCloseCallback.InvokeAsync();
        }
        async Task OnNavigationBackClick()
        {
            _isPanelOpen = false;
            await OnNavigationBackCallback.InvokeAsync();
        }
        

        async Task OnSearchClear()
        {
            _searchField.Value = string.Empty;
            await LoadScheduleGroups();

        }


        public async Task Show(Guid? schduleId, bool showNavigation = false)
        {
            if (schduleId != null)
            {
                _sheduleId = (Guid)schduleId;
                showBackButton = showNavigation;
                await LoadScheduleGroups();
                _isPanelOpen = true;
                StateHasChanged();
            }
        }

        private async Task LoadScheduleGroups()
        {
            try
            {
                _isBusy = true;
                string _searchString = string.Empty;
                if (_searchField != null)
                    _searchString = _searchField.Value;

                _addGroupsListError = string.Empty;
                _groups = null;
                _groups =  (await ScheduleGroupClient.SearchGroups(_sheduleId, _searchString)).Data.OrderBy(x=>x.Name).ToList();
                if (_groups == null)
                {
                    _addGroupsListError = "Error searching Groups. Please try again.";
                    _groups = new List<Group>();
                }
               
            }
            catch (Exception ex)
            {
                _addGroupsListError = "Error loading Add Schedules Groups. Please try again.";
            }
            finally
            {
                _isBusy = false;
                
            }
        }
       

        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<Group>("Group", x => x.Name) { MinWidth = 30, MaxWidth = 500, Index = 0, OnColumnClick = OnGroupColumnClick, IsResizable = true });
        }

        private void OnGroupColumnClick(IDetailsRowColumn<Group> column)
        {
            if (column.IsSortedDescending && column.IsSorted)
                column.IsSorted = !column.IsSorted;

            if (column.IsSorted)
            {
                _groups = new List<Group>(column.IsSorted ? _groups.OrderBy(x => x.Name) : _groups.OrderByDescending(x => x.Name));
                column.IsSortedDescending = !column.IsSortedDescending;
            }
            else
            {
                _groups = new List<Group>(column.IsSorted ? _groups.OrderBy(x => x.Name) : _groups.OrderByDescending(x => x.Name));
                column.IsSorted = !column.IsSorted;
                column.IsSortedDescending = false;
            }
            StateHasChanged();
        }


        private async Task OnScheduleGroupCreate()
        {
            string _name = string.Empty;
            bool isAdded = false;
            try
            {
                _isBusy = true;
                
                foreach(var item in _selectedGroup.SelectedItems)
                {
                    _name = item.Name;
                    var response = await ScheduleGroupClient.CreateSheduleGroup(_sheduleId, item);
                    isAdded = true;
                }
            }
            catch(Exception ex) 
            {
                isAdded = false;
                _addGroupsListError = "Error in adding group of " + _name + " already exist in records";
            }
            finally
            {
                _isBusy = false;
                if(isAdded)
                    await OnNavigationBackClick();
            }
        }

        async Task CloseHandler()
        {
            _isPanelOpen = false;
            await Task.CompletedTask;
        }


    }
}
