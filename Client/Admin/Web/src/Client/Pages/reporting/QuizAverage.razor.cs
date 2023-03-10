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
    public partial class QuizAverage
    {
        [Inject] private IReportingClient ReportingClient { get; set; }

        private readonly List<DetailsRowColumn<QuizAverageReportItemViewModel>> _columns = new List<DetailsRowColumn<QuizAverageReportItemViewModel>>();

        private bool _isBusy = false;

        private bool _showErrorMessage = false;

        private bool _showInfoMessage => (!_isBusy && (_reportData == null || _reportData.Count == 0));

        private bool _canShowGrid => !_isBusy && !_showErrorMessage && !_showInfoMessage;

        private List<QuizAverageReportItemViewModel> _reportData;

        protected override async Task OnInitializedAsync()
        {
            AddColumns();
            _reportData = await LoadReport();
        }

        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<QuizAverageReportItemViewModel>("Quiz Name", x => x.QuizName) { MinWidth = 30, MaxWidth = 300, Index = 0, OnColumnClick = OnQuizNameColumnClick, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizAverageReportItemViewModel>("% done", x => x.DonePercentageFormat) { Index = 4, MinWidth = 50, MaxWidth = 100, OnColumnClick = OnDonePercentageColumnClick, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizAverageReportItemViewModel>("Avg Time", x => x.AvergageTime) { Index = 5, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizAverageReportItemViewModel>("Groups", x => x.GroupCount) { Index = 6, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizAverageReportItemViewModel>("Users", x => x.UserCount) { Index = 6, MinWidth = 50, MaxWidth = 100, IsResizable = true });
        }

        private void OnQuizNameColumnClick(IDetailsRowColumn<QuizAverageReportItemViewModel> column)
        {
            _reportData = new List<QuizAverageReportItemViewModel>(column.IsSorted ? _reportData.OrderBy(x => x.QuizName) : _reportData.OrderByDescending(x => x.QuizName));
            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OnDonePercentageColumnClick(IDetailsRowColumn<QuizAverageReportItemViewModel> column)
        {
            _reportData = new List<QuizAverageReportItemViewModel>(column.IsSorted ? _reportData.OrderBy(x => x.DonePercentageFormat) : _reportData.OrderByDescending(x => x.DonePercentageFormat));
            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        public async Task<List<QuizAverageReportItemViewModel>> LoadReport()
        {
            var list = new List<QuizAverageReportItemViewModel>();
            try
            {
                _isBusy = true;
                _showErrorMessage = false;
                var response = await ReportingClient.GetQuizAverageReport();
                if (!response.IsOK())
                {
                    _showErrorMessage = true;
                    list = new List<QuizAverageReportItemViewModel>();
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

        private List<QuizAverageReportItemViewModel> MapReportData(IList<QuizAverageReportItem> quizAverageReportItems)
        {
            var list = new List<QuizAverageReportItemViewModel>();

            return quizAverageReportItems.Select(m => new QuizAverageReportItemViewModel
            {
                QuizName = m.QuizName ?? "",
                DonePercentage = m.DonePercentage ?? 0,
                AvergageTime = m.AvergageTime ?? "",
                GroupCount = m.GroupCount,
                UserCount = m.UserCount
            }).ToList();
        }
    }
}
