using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;

namespace SwiftCAPS.Admin.Web.Client.Pages.reporting
{
    public partial class GroupAverage
    {
        [Inject] private IReportingClient ReportingClient { get; set; }

        private readonly List<DetailsRowColumn<GroupAverageReportItemViewModel>> _columns = new List<DetailsRowColumn<GroupAverageReportItemViewModel>>();

        private bool _isBusy = false;

        private bool _showErrorMessage = false;

        private bool _showInfoMessage => (!_isBusy && (_reportData == null || _reportData.Count == 0));

        private bool _canShowGrid => !_isBusy && !_showErrorMessage && !_showInfoMessage;

        private List<GroupAverageReportItemViewModel> _reportData;

        protected override async Task OnInitializedAsync()
        {
            AddColumns();
            _reportData = await LoadReport();
        }

        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<GroupAverageReportItemViewModel>("Group Name", x => x.GroupName) { MinWidth = 30, MaxWidth = 300, Index = 0, OnColumnClick = OnGroupNameColumnClick, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupAverageReportItemViewModel>("Quizzes", x => x.QuizCount) { Index = 1, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupAverageReportItemViewModel>("Avg % done", x => x.AverageDonePercentageFormat) { Index = 2, MinWidth = 50, OnColumnClick = OnDonePercentageColumnClick, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupAverageReportItemViewModel>("Avg Time", x => x.AvergageTime) { Index = 3, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<GroupAverageReportItemViewModel>("Users", x => x.UserCount) { Index = 4, MinWidth = 50, MaxWidth = 100,  IsResizable = true });
        }

        private void OnGroupNameColumnClick(IDetailsRowColumn<GroupAverageReportItemViewModel> column)
        {
            _reportData = new List<GroupAverageReportItemViewModel>(column.IsSorted ? _reportData.OrderBy(x => x.GroupName) : _reportData.OrderByDescending(x => x.GroupName));
            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OnDonePercentageColumnClick(IDetailsRowColumn<GroupAverageReportItemViewModel> column)
        {
            _reportData = new List<GroupAverageReportItemViewModel>(column.IsSorted ? _reportData.OrderBy(x => x.AverageDonePercentageFormat) : _reportData.OrderByDescending(x => x.AverageDonePercentageFormat));
            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        public async Task<List<GroupAverageReportItemViewModel>> LoadReport()
        {
            var list = new List<GroupAverageReportItemViewModel>();
            try
            {
                _isBusy = true;
                _showErrorMessage = false;
                var response = await ReportingClient.GetGroupAverageReport();
                if (!response.IsOK())
                {
                    _showErrorMessage = true;
                    list = new List<GroupAverageReportItemViewModel>();
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

        private List<GroupAverageReportItemViewModel> MapReportData(IList<GroupAverageReportItem> groupAverageReportItemViewModel)
        {
            var list = new List<GroupAverageReportItemViewModel>();

            return groupAverageReportItemViewModel.Select(m => new GroupAverageReportItemViewModel
            {
                GroupName = m.GroupName ?? "",
                QuizCount = m.QuizCount,
                AverageDonePercentage = m.AverageDonePercentage?? 0,
                AvergageTime = m.AvergageTime ?? "",
                UserCount = m.UserCount
            }).ToList();
        }
    }
}
