using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwiftCaps.Client.Shared.Models;
using SwiftCaps.Models.Requests;
using SwiftCAPS.Web.Shared.Clients;
using SwiftCAPS.Web.Shared.State;
using LeaderBoardM = SwiftCaps.Models.Models.LeaderBoard;

namespace SwiftCAPS.Web.Client.Pages
{
    [Authorize(Roles = nameof(RoleConstants.User))]
    public partial class LeaderBoard
    {
        protected IList<LeaderBoardM> LeaderBoards { get; private set; }
        [Inject] public IReportingClient ReportingClient { get; set; }
        [Inject] public IUserClient UserClient { get; set; }
        protected string ErrorMessage { get; set; }
        [Inject] public UserState UserState { get; set; }
        //[Inject]
        public ILogger Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await GetLeaderBoards();
            }
            catch (UnauthorizedAccessException)
            {
                ErrorMessage = "Please relogin.";
            }
            catch (Exception e)
            {
                ErrorMessage = "There was an error retrieving leaderboard, please try again.";
                Logger?.LogError(e, ErrorMessage);
            }
        }

        private async Task GetLeaderBoards()
        {
            if (UserState.User is null)
            {
                var userResponse = await UserClient.GetOrCreateUser();
                if (!userResponse.IsOK())
                {
                    throw new Exception(string.Join(',', userResponse.Errors));
                }
                UserState.SetUser(userResponse.Data);
            }

            var response = await ReportingClient.GetLeaderboard(new UserQuizRequest
            {
                ClientLocalDateTime = DateTimeOffset.Now,
                UserId = UserState.User.Id
            });
            if (!response.IsOK())
            {
                throw new Exception(string.Join(',', response.Errors));
            }
            LeaderBoards = response.Data;

        }
    }
}
