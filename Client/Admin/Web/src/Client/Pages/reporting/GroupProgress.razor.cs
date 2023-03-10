using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;

namespace SwiftCAPS.Admin.Web.Client.Pages.reporting
{
    public partial class GroupProgress
    {
        [Inject] private IReportingClient ReportingClient { get; set; }

        private readonly List<DetailsRowColumn<GroupProgressReportItemViewModel>> _columns = new List<DetailsRowColumn<GroupProgressReportItemViewModel>>();

        private bool _isBusy = false;

        private bool _showErrorMessage = false;

        private bool _showInfoMessage => (!_isBusy && (_reportData == null || _reportData.Count == 0));

        private bool _canShowGrid => !_isBusy && !_showErrorMessage && !_showInfoMessage;

        private List<GroupProgressReportItemViewModel> _reportData;

        protected override async Task OnInitializedAsync()
        {
            AddColumns();
            _reportData = await LoadReport();
        }

        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<GroupProgressReportItemViewModel>("Group", x => x.GroupName) { MinWidth = 30, MaxWidth = 300, Index = 0, OnColumnClick = OnGroupNameColumnClick, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupProgressReportItemViewModel>("Quiz", x => x.QuizName) { Index = 1, MinWidth = 30, MaxWidth = 500, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupProgressReportItemViewModel>("Recurrence", x => x.RecurrenceDisplayText) { Index = 2, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupProgressReportItemViewModel>("Sequence", x => x.Sequence) { Index = 3, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupProgressReportItemViewModel>("% Done", x => x.DonePercentageFormat) { Index = 4, MinWidth = 50, MaxWidth = 100, OnColumnClick = OnDonePercentageColumnClick, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupProgressReportItemViewModel>("Avg Time", x => x.AvergageTime) { Index = 5, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupProgressReportItemViewModel>("Users", x => x.UserCount) { Index = 6, MinWidth = 50, MaxWidth = 100, IsResizable = true });
        }

        private void OnGroupNameColumnClick(IDetailsRowColumn<GroupProgressReportItemViewModel> column)
        {
            _reportData = new List<GroupProgressReportItemViewModel>(column.IsSorted ? _reportData.OrderBy(x => x.GroupName) : _reportData.OrderByDescending(x => x.GroupName));
            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OnDonePercentageColumnClick(IDetailsRowColumn<GroupProgressReportItemViewModel> column)
        {
            _reportData = new List<GroupProgressReportItemViewModel>(column.IsSorted ? _reportData.OrderBy(x => x.DonePercentageFormat) : _reportData.OrderByDescending(x => x.DonePercentageFormat));
            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private async Task<List<GroupProgressReportItemViewModel>> LoadReport()
        {
            var list = new List<GroupProgressReportItemViewModel>();
            try
            {
                _isBusy = true;
                _showErrorMessage = false;
                var response = await ReportingClient.GetGroupPogressReport(new AdminReportingRequest
                {
                    ClientLocalDateTime = DateTime.Now
                });
                if (!response.IsOK())
                {
                    _showErrorMessage = true;
                    list = new List<GroupProgressReportItemViewModel>();
                }
                else
                {
                    list = MapReportData(response.Data);
                }
            }
            catch
            {
                _showErrorMessage = true;
            }
            finally
            {
                _isBusy = false;
            }
            return list;
        }

        private List<GroupProgressReportItemViewModel> MapReportData(IList<GroupProgressReportItem> groupProgressReportItems)
        {
            var list = new List<GroupProgressReportItemViewModel>();

            return groupProgressReportItems.Select(m => new GroupProgressReportItemViewModel
            {
                GroupName = m.GroupName ?? "",
                QuizName = m.QuizName ?? "",
                Recurrence = m.Recurrence,
                Sequence = m.Sequence ?? "",
                DonePercentage = m.DonePercentage ?? 0,
                AvergageTime = m.AvergageTime ?? "",
                UserCount = m.UserCount
            }).ToList();
        }
    }
}
