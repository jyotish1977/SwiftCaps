using System.Linq;
using Shouldly;
using SwiftCAPS.Mobile.UnitTest.Infrastructure;
using SwiftCaps.ViewModels;
using TechTalk.SpecFlow;
using Unity;

namespace SwiftCAPS.Mobile.UnitTest.Tests
{
    [Binding]
    public sealed class QuizTrackerTestSteps : StepBase
    {
        private QuizTrackerPageViewModel _viewModel => GetCurrentViewModel<QuizTrackerPageViewModel>();

        public QuizTrackerTestSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [Then(@"I can see the Quiz tracker with (.*) weekly test and (.*) monthly test")]
        public void ThenICanSeeTheQuizTrackerWithTheUsernameAndWeeklyTestNumberAndMonthlyTestNumber(int weekly,
            int monthly)
        {
            _viewModel.GetLeaderBoardCommand?.WaitHandle.WaitOne();
            _viewModel.GetLeaderBoardCommand.Execute();
            _viewModel.GetLeaderBoardCommand?.WaitHandle.WaitOne();
            var leaderBoards = _viewModel.LeaderBoardList.FirstOrDefault();
            leaderBoards.ShouldNotBeNull();

            leaderBoards.WeeklyQuizReports.Count.ShouldBe(weekly);
            leaderBoards.MonthlyQuizReports.Count.ShouldBe(monthly);
        }

        [Then(@"I should see a list of users")]
        public void ThenIShouldSeeAListOfUsers()
        {
            _viewModel.LeaderBoardList.Any().ShouldBeTrue();
        }
    }
}