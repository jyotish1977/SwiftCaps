using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.Models;
using LeaderBoardEntity = SwiftCaps.Models.Models.LeaderBoard;

namespace SwiftCAPS.Admin.Web.Client.Pages.reporting
{
    public partial class LeaderBoard
    {
        [Inject] private IGroupClient GroupClient { get; set; }
        [Inject] private IReportingClient ReportingClient { get; set; }

        private readonly List<DetailsRowColumn<ReportingLeaderboardViewModel>> _columns = new List<DetailsRowColumn<ReportingLeaderboardViewModel>>();
       
        private Guid _selectedGroup;

        private bool _isBusy = false;

        private bool _isLeaderboardLoading = false;

        private string _leaderboardError;
        private bool _showErrorMessage => !string.IsNullOrEmpty(_leaderboardError);


        private string _leaderboardInfoMessage;
        private bool _showInfoMessage => !string.IsNullOrEmpty(_leaderboardInfoMessage);

        private bool _canShowGrid => !_isBusy && string.IsNullOrEmpty(_leaderboardError) && string.IsNullOrEmpty(_leaderboardInfoMessage);

        private List<GroupViewModel> _groups;

        private List<ReportingLeaderboardViewModel> _leaderboards;

        protected override async Task OnInitializedAsync()
        {
            AddColumns();
            _groups = await LoadGroups();

            if (_groups.Count() > 0)
            {
                _leaderboards = await LoadLeaderboard(_groups.First().Id);
            }
        }

        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<ReportingLeaderboardViewModel>("User", x => x.User) { MinWidth = 30, MaxWidth = 500, Index = 0, OnColumnClick = OnUserColumnClick, IsResizable = true });
            _columns.Add(new DetailsRowColumn<ReportingLeaderboardViewModel>("Monthly", x => x.MonthlyCompletedDisplayText) { Index = 1, MinWidth = 50, MaxWidth = 100, IsResizable = true });
            _columns.Add(new DetailsRowColumn<ReportingLeaderboardViewModel>("Weekly", x => x.WeeklyCompletedDisplayText) { Index = 2, MinWidth = 50, MaxWidth = 100, IsResizable = true });
        }

        private void OnUserColumnClick(IDetailsRowColumn<ReportingLeaderboardViewModel> column)
        {
            _leaderboards = new List<ReportingLeaderboardViewModel>(column.IsSorted ? _leaderboards.OrderBy(x => x.User) : _leaderboards.OrderByDescending(x => x.User));
            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        public async Task OnGroupClick(Guid groupId)
        {
            _leaderboards = await LoadLeaderboard(groupId);
        }

        public async Task<List<ReportingLeaderboardViewModel>> LoadLeaderboard(Guid groupId)
        {
            var list = new List<ReportingLeaderboardViewModel>();
            try
            {
                _isLeaderboardLoading = true;
                _leaderboardError = string.Empty;
                _selectedGroup = groupId;
                var response = await ReportingClient.GetLeaderBoard(groupId, new AdminReportingRequest
                {
                    ClientLocalDateTime = DateTime.Now
                });
                if (!response.IsOK())
                {
                    _leaderboardError = "Error retrieving leader board data.  Please try again";
                    list = new List<ReportingLeaderboardViewModel>();
                }
                else
                {
                    list = MapLeaderboard(response.Data);
                }
            }
            catch
            {
                _leaderboardError = "Error retrieving leader board data.  Please try again";
            }
            finally
            {
                _isLeaderboardLoading = false;
            }
            return list;
        }

        private List<ReportingLeaderboardViewModel> MapLeaderboard(IList<LeaderBoardEntity> leaderboards)
        {
            var list = new List<ReportingLeaderboardViewModel>();

            return leaderboards.Select(m => new ReportingLeaderboardViewModel
            {
                UserId = m.UserId,
                User = m.UserName,
                MonthlyCompleted = m.MonthlyQuizReports?.FirstOrDefault()?.IsCompleted ?? false,
                WeeklyCompleted = m.WeeklyQuizReports?.FirstOrDefault()?.IsCompleted ?? false
            }).ToList();
        }

        private async Task<List<GroupViewModel>> LoadGroups()
        {
            var list = new List<GroupViewModel>();
            try
            {
                _isBusy = true;
                _leaderboardError = string.Empty;
                var response = await GroupClient.GetGroups();
                if (!response.IsOK())
                {
                    _leaderboardError = "Error retrieving Group(s). Please try again";
                    list = new List<GroupViewModel>();
                }
                else
                {
                    list = MapGroups(response.Data);
                    if(list.Count == 0)
                    {
                        _leaderboardInfoMessage = "No Group(s) found.";
                    }
                }
            }
            catch
            {
                _leaderboardError = "Error retrieving Group(s). Please try again";
            }
            finally
            {
                _isBusy = false;
            }
            return list;
        }

        private List<GroupViewModel> MapGroups(IList<Group> groups)
        {
            var list = new List<GroupViewModel>();

            return groups.Select(m => new GroupViewModel
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();
        }
    }
}
