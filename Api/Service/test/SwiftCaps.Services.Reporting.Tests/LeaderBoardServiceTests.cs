using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using SwiftCaps.Models.Requests;
using Xunit;
using SwiftCaps.Helpers.Tests;

namespace SwiftCaps.Services.Reporting.Tests
{
    public class LeaderBoardServiceTests
    {
        [Fact]
        public async Task LeaderBoardService_GetLeaderBoard_ShouldReturnCorrectUsers()
        {
            var context = ContextHelper.GenerateSwiftcapsContext();
            var leaderBoardService = new LeaderBoardService(context);
            var selectedUser = context.Users.First();
            var leaderBoards = await leaderBoardService.GetLeaderBoard(new UserQuizRequest
            {
                UserId = selectedUser.Id,
                ClientLocalDateTime = new DateTimeOffset(new DateTime(2020, 9, 20))
            });

            leaderBoards.Data.Count.ShouldBe(4);
        }

        [Theory]
        [InlineData("2020-9-20",1)]
        [InlineData("2020-9-13 08:30:00",2)]
        public async Task LeaderBoardService_GetLeaderBoard_ShouldReturnCorrectWeeklyShift(DateTime date, int expectedWeekly)
        {
            var context = ContextHelper.GenerateSwiftcapsContext();
            var leaderBoardService = new LeaderBoardService(context);
            var selectedUser = context.Users.First();
            var leaderBoards = await leaderBoardService.GetLeaderBoard(new UserQuizRequest
            {
                UserId = selectedUser.Id,
                ClientLocalDateTime = new DateTimeOffset(date)
            });

            foreach (var leaderBoard in leaderBoards.Data)
            {
                leaderBoard.WeeklyQuizReports.Count.ShouldBe(expectedWeekly);
            }
        }

        [Theory]
        [InlineData("2020-9-20", 1)]
        [InlineData("2020-8-31 08:30:00", 2)]
        public async Task LeaderBoardService_GetLeaderBoard_ShouldReturnCorrectMonthlyShiftShift(DateTime date, int expectedMonthly)
        {
            var context = ContextHelper.GenerateSwiftcapsContext();
            var leaderBoardService = new LeaderBoardService(context);
            var selectedUser = context.Users.First();
            var leaderBoards = await leaderBoardService.GetLeaderBoard(new UserQuizRequest
            {
                UserId = selectedUser.Id,
                ClientLocalDateTime = new DateTimeOffset(date)
            });

            foreach (var leaderBoard in leaderBoards.Data)
            {
                leaderBoard.MonthlyQuizReports.Count.ShouldBe(expectedMonthly);
            }
        }
    }
}
