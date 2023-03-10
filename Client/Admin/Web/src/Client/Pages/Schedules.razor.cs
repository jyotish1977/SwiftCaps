using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Client.Components;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using SwiftCAPS.Blazor.Components;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace SwiftCAPS.Admin.Web.Client.Pages
{
    [Authorize]
    public partial class Schedules
    {
        [Inject] public IScheduleClient ScheduleClient { get; set; }

        private readonly List<DetailsRowColumn<ScheduleSummaryItem>> _columns = new();
        private Selection<ScheduleSummaryItem> _selectedScheduleSummary;
        private bool _isBusy = false;
        private string _scheduleListError;
        private bool _showErrorMessage => !string.IsNullOrEmpty(_scheduleListError);
        private bool _canShowGrid => !_isBusy && string.IsNullOrEmpty(_scheduleListError);
        public List<ScheduleSummaryItem> ScheduleList;
        private ICommand _buttonCommand;
        private List<CommandBarItem> _commandItems;

        private ScheduleDetail detailsPanel;
        private UpsertSchedule upsertPanel;
        private DeletePanel deletePanel;

        public Selection<ScheduleSummaryItem> SelectedScheduleSummary
        {
            get => _selectedScheduleSummary;
            set
            {
                _selectedScheduleSummary = value;
            }
        }

        private async Task OnScheduleItemClick(ScheduleSummaryItem item)
        {
            if (SelectedScheduleSummary.SelectedItems.Any())
            {
                await detailsPanel.Show(SelectedScheduleSummary.SelectedItems.First().Id);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            AddColumns();
            ConfigureCommandBar();
            SelectedScheduleSummary = new Selection<ScheduleSummaryItem>();
            await LoadSchedules();

            SelectedScheduleSummary.SelectionChanged.Subscribe(m =>
            {
                InvokeAsync(StateHasChanged);
            });
        }

        private async Task OnDeleteCloseHandler()
        {
            await LoadSchedules();
        }

        private async Task<HttpResponseMessage> DeleteHandler()
        {
            var selectedId = SelectedScheduleSummary?.SelectedItems?.FirstOrDefault()?.Id;
            return await ScheduleClient.DeleteSchedule(selectedId.Value);
        }

        public void ConfigureCommandBar()
        {
            _buttonCommand = new RelayCommand(async (item) =>
            {
                if (item != null)
                {
                    await CommandItemClick((string)item);
                }
            });

            _commandItems = new List<CommandBarItem> {
                new CommandBarItem() {  Text= "Add Schedule", IconName="Add", Key=CommandItem.Add.ToString()},
                new CommandBarItem() {  Text= "Edit Schedule", Disabled=true, IconName="Edit", Key=CommandItem.Edit.ToString()},
                new CommandBarItem() {  Text= "Delete Schedule",Disabled=true, IconName="Delete", Key=CommandItem.Delete.ToString()}
            };
        }

        private async Task CommandItemClick(string item)
        {
            if (item == CommandItem.Delete.ToString())
            {
                await deletePanel.Show("Schedule");
            }
            if (item == CommandItem.Add.ToString())
            {
                await upsertPanel.Show();
            }
            if (item == CommandItem.Edit.ToString())
            {
                await upsertPanel.Show(SelectedScheduleSummary.SelectedItems.First().Id);
            }
        }

        private async Task LoadSchedules()
        {
            try
            {
                _isBusy = true;
                _scheduleListError = string.Empty;
                var response = await ScheduleClient.GetSchedules();
                if (!response.IsOK())
                {
                    _scheduleListError = "Error loading Schedules. Please try again.";
                    ScheduleList = new List<ScheduleSummaryItem>();
                }
                else
                    ScheduleList = MapSchedules(response.Data);
            }
            catch
            {
                _scheduleListError = "Error loading Schedules. Please try again.";
            }
            finally
            {
                _isBusy = false;
            }
        }

        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<ScheduleSummaryItem>("Quiz", x => x.QuizName) { MinWidth = 30, MaxWidth = 500, Index = 0, OnColumnClick = OnQuizColumnClick, IsResizable = true, });
            _columns.Add(new DetailsRowColumn<ScheduleSummaryItem>("Recurrence", x => x.Recurrence) { Index = 1, MinWidth = 30, MaxWidth = 80, IsResizable = true });
            _columns.Add(new DetailsRowColumn<ScheduleSummaryItem>("Start date", x => x.Start) { Index = 2, MinWidth = 30, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<ScheduleSummaryItem>("End date", x => x.End) { Index = 3, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<ScheduleSummaryItem>("Groups", x => x.GroupCount) { Index = 4, MaxWidth = 80, IsResizable = true });
        }

        private void OnQuizColumnClick(IDetailsRowColumn<ScheduleSummaryItem> column)
        {
            if (column.IsSortedDescending && column.IsSorted)
                column.IsSorted = !column.IsSorted;

            if (column.IsSorted)
            {

                ScheduleList = new List<ScheduleSummaryItem>(column.IsSorted ? ScheduleList.OrderBy(x => x.QuizName) : ScheduleList.OrderByDescending(x => x.QuizName));
                column.IsSortedDescending = !column.IsSortedDescending;
            }
            else
            {
                ScheduleList = new List<ScheduleSummaryItem>(column.IsSorted ? ScheduleList.OrderBy(x => x.QuizName) : ScheduleList.OrderByDescending(x => x.QuizName));
                column.IsSorted = !column.IsSorted;
                column.IsSortedDescending = false;
            }
            StateHasChanged();
        }

        private List<ScheduleSummaryItem> MapSchedules(IList<ScheduleSummary> schedules)
        {
            return schedules.Select(s => new ScheduleSummaryItem
            {
                Id = s.Id,
                QuizName = s.QuizName,
                Recurrence = s.Recurrence,
                Start = (s.Start == null) ? "" : s.Start?.ToString("dd/MM/yyyy"),
                End = (s.End == null) ? "" : s.End?.ToString("dd/MM/yyyy"),
                GroupCount = s.GroupCount
            }).ToList();
        }

        private async Task OnDetailsCloseHandler()
        {
            await LoadSchedules();
        }

        private async Task OnDetailsPanelCloseHandler()
        {
            await LoadSchedules();
        }

        private async Task OnUpsertCloseHandler()
        {
            await LoadSchedules();
        }

    }
}
