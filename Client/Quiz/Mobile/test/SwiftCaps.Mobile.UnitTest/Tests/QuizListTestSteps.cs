using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using SwiftCaps.Data.Context;
using SwiftCaps.Fake.Helpers;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCaps.ViewModels;
using SwiftCAPS.Mobile.UnitTest.Infrastructure;
using TechTalk.SpecFlow;
using Unity;
using Xamarin.Forms.Internals;
using Xamariners.Core.Common.Helpers;
using Xamariners.RestClient.Helpers;

namespace SwiftCAPS.Mobile.UnitTest.Tests
{
    [Binding]
    public class UserQuizTestSteps : StepBase
    {
        private SwiftCapsContext _swiftCapsContext => App.Container.Resolve<SwiftCapsContext>();
        private QuizListPageViewModel _viewModel => GetCurrentViewModel<QuizListPageViewModel>();

        public DateTimeOffset LocalDateTime { get; set; }


        public UserQuizTestSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            LocalDateTime = DateTimeOffset.Now;
        }

        [Then(@"I can see a ""([^""]*)"" ""([^""]*)"" ""([^""]*)"" quiz card with title ""([^""]*)""")]
        public void ThenICanSeeAQuizCardWithTitle(string completeString, string current, Recurrence recurrence,
            string name)
        {
            _viewModel.UserQuizzes.ShouldNotBeNull();
            _viewModel.UserQuizzes.ShouldNotBeEmpty();

            DateTimeOffset expiryDate;

            switch (current.ToLower())
            {
                case "previous":
                    expiryDate = recurrence == Recurrence.Weekly
                        ? LocalDateTime.AddDays(-7).LastDayOfWeek(false)
                        : LocalDateTime.AddMonths(-1).LastDayOfMonth(false);
                    break;
                case "current":
                    expiryDate = recurrence == Recurrence.Weekly
                        ? LocalDateTime.LastDayOfWeek(false)
                        : LocalDateTime.LastDayOfMonth(false);
                    break;
                case "next":
                    expiryDate = recurrence == Recurrence.Weekly
                        ? LocalDateTime.AddDays(7).LastDayOfWeek(false)
                        : LocalDateTime.AddMonths(1).LastDayOfMonth(false);
                    break;
                default:
                    throw new ArgumentException(nameof(current));

            }

            var complete = completeString.ToLower() == "completed";
            
            _viewModel?.UserQuizzes?.Any(x =>
            {
                var quizExpiryDate = x.Expiry.ToString("d");
                var completedHasValue = x.Completed.HasValue;
                var quizName = x.Schedule.Quiz.Name;
                var quizRecurrence = x.Schedule.Recurrence;
                var any = completedHasValue == complete && quizName == name && quizRecurrence == recurrence &&
                          quizExpiryDate == expiryDate.ToString("d");

                return any;
            }).ShouldBeTrue(_viewModel?.UserQuizzes?.Count().ToString());
        }

        [Given(@"I completed the ""([^""]*)"" ""([^""]*)"" Quiz")]
        [When(@"I completed the ""([^""]*)"" ""([^""]*)"" Quiz")]
        [Then(@"I completed the ""([^""]*)"" ""([^""]*)"" Quiz")]
        public void GivenICompletedTheQuiz(string status, Recurrence recurrence)
        {
            var quiz = GetQuizByRecurrence(_viewModel.UserQuizzes.ToList(), status.ToLower(), recurrence);

            quiz.ShouldNotBeNull(_viewModel.UserQuizzes.Count.ToString());
            quiz.Completed = DateTime.UtcNow;
            quiz.Completed.ShouldNotBeNull();

            _viewModel.UserQuizzes.AddReplace(quiz);

            _viewModel.UserQuizzes.Single(x => x.Id == quiz.Id).Completed.ShouldNotBeNull();
        }

        [Given(@"The ""(.*)"" ""(.*)"" is incomplete")]
        [When(@"The ""(.*)"" ""(.*)"" is incomplete")]
        [Then(@"The ""(.*)"" ""(.*)"" is incomplete")]
        public void ThenTheIsIncomplete(string status, Recurrence recurrence)
        {
            _viewModel.UserQuizzes.Any().ShouldBeTrue();

            var quiz = GetQuizByRecurrence(_viewModel.UserQuizzes.ToList(), status.ToLower(), recurrence);

            quiz.ShouldNotBeNull();
            quiz.Completed = null;
            quiz.Completed.ShouldBeNull();
        }


        [When(@"I tap on the ""([^""]*)"" ""([^""]*)"" Quiz card")]
        public void ThenITapOnTheQuizCard(string status, Recurrence recurrence)
        {
            _viewModel.UserQuizzes.Any().ShouldBeTrue();

            var quiz = GetQuizByRecurrence(_viewModel.UserQuizzes.ToList(), status.ToLower(), recurrence);

            quiz.ShouldNotBeNull();

            _viewModel.GoToQuizCommand.Execute(quiz);

            if (_viewModel.GoToQuizCommand is { IsValid: true })
                _viewModel.GoToQuizCommand?.WaitHandle.WaitOne();
        }

        [When(@"I tap on the expired ""([^""]*)"" ""([^""]*)"" Quiz card")]
        public void ThenITapOnTheExpiredQuizCard(string status, Recurrence recurrence)
        {
            _viewModel.UserQuizzes.Any().ShouldBeTrue();

            var quiz = GetQuizByRecurrence(_viewModel.UserQuizzes.ToList(), status.ToLower(), recurrence);

            quiz.ShouldNotBeNull();
           
            _viewModel.GoToQuizCommand.Execute(quiz);

            if (_viewModel.GoToQuizCommand is { IsValid: true })
                _viewModel.GoToQuizCommand?.WaitHandle.WaitOne();
        }

        public UserQuiz GetQuizByRecurrence(List<UserQuiz> userQuizzes, string isCompleted, Recurrence recurrence)
        {
            UserQuiz quiz;

            switch (isCompleted.ToLower())
            {
                case "previous":
                    if (recurrence == Recurrence.Weekly)
                    {
                        quiz = userQuizzes.FirstOrDefault(x =>
                            x.Expiry.ToString("d") == LocalDateTime.AddDays(-7).LastDayOfWeek(false).ToString("d") && x.Schedule.Recurrence == Recurrence.Weekly)!;
                    }
                    else
                    {
                        quiz = userQuizzes.FirstOrDefault(x =>
                            x.Expiry.ToString("d") == LocalDateTime.AddMonths(-1).LastDayOfMonth(false).ToString("d") && x.Schedule.Recurrence == Recurrence.Monthly)!;
                    }
                    break;
                case "current":
                    if (recurrence == Recurrence.Weekly)
                    {
                        var d = LocalDateTime.LastDayOfWeek(false);

                        quiz = userQuizzes.FirstOrDefault(x =>
                            x.Expiry.ToString("d") == LocalDateTime.LastDayOfWeek(false).ToString("d") && x.Schedule.Recurrence == Recurrence.Weekly)!;
                    }
                    else
                    {
                        quiz = userQuizzes.FirstOrDefault(x =>
                            x.Expiry.ToString("d") == LocalDateTime.LastDayOfMonth(false).ToString("d") && x.Schedule.Recurrence == Recurrence.Monthly)!;
                    }
                    break;
                case "next":
                    if (recurrence == Recurrence.Weekly)
                    {
                        quiz = userQuizzes.FirstOrDefault(x =>
                            x.Expiry.ToString("d") == LocalDateTime.AddDays(7).LastDayOfWeek(false).ToString("d") && x.Schedule.Recurrence == Recurrence.Weekly)!;
                    }
                    else
                    {
                        quiz = userQuizzes.FirstOrDefault(x =>
                            x.Expiry.ToString("d") == LocalDateTime.AddMonths(1).LastDayOfMonth(false).ToString("d") && x.Schedule.Recurrence == Recurrence.Monthly)!;
                    }
                    break;
                default:
                    throw new Exception("Quiz parameters were incorrect.");
            }

            return quiz;
        }

        #region Dates


        [Given(@"today is the last day of the month between (.*) and (.*)")]
        [When(@"today is the last day of the month between (.*) and (.*)")]
        [Then(@"today is the last day of the month between (.*) and (.*)")]
        public void GivenTodayIsTheLastDayOfTheMonthBetweenAnd(string time1, string time2)
        {
            var avg = GetAverage(time1, time2);

            var monthDate = LocalDateTime.LastDayOfMonth(false).Add(TimeSpan.Parse(time1)).AddHours(avg.Hours);

            var lastDayStartTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time2));

            monthDate.ShouldBeInRange(lastDayStartTime, lastDayEndTime);

            SetCurrentDateTimeOffset(monthDate);
        }

        [Given(@"today is the last day of the month and sunday between (.*) and (.*)")]
        [When(@"today is the last day of the month and sunday between (.*) and (.*)")]
        [Then(@"today is the last day of the month and sunday between (.*) and (.*)")]
        public void GivenTodayIsTheLastDayOfTheMonthAndSundayBetweenAnd(string time1, string time2)
        {
            var avg = GetAverage(time1, time2);

            //Mock datetime to be sunday and last date of month
            var dateTime = new DateTime(2021, 2, 28);
            var dateTimeOffset = new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime));
            var monthDate = dateTimeOffset.LastDayOfMonth(false).Add(TimeSpan.Parse(time1)).AddHours(avg.Hours);

            var lastDayStartTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time2));

            monthDate.ShouldBeInRange(lastDayStartTime, lastDayEndTime);
            monthDate.DayOfWeek.ShouldBe(DayOfWeek.Sunday);

            SetCurrentDateTimeOffset(monthDate);

        }

        [Given(@"today is not the last day of the month between (.*) and (.*)")]
        [When(@"today is not the last day of the month between (.*) and (.*)")]
        [Then(@"today is not the last day of the month between (.*) and (.*)")]
        public void GivenTodayIsNotTheLastDayOfTheMonthBetweenAndString(string time1, string time2)
        {

            var monthDate = LocalDateTime;

            if (monthDate.Day == monthDate.LastDayOfMonth(false).Day)
            {
                monthDate = monthDate.AddDays(1);
            }

            var lastDayStartTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time2));

            monthDate.ShouldNotBeInRange(lastDayStartTime, lastDayEndTime);

            SetCurrentDateTimeOffset(monthDate);
        }

        [Given(@"today is sunday between (.*) and (.*)")]
        [When(@"today is sunday between (.*) and (.*)")]
        [Then(@"today is sunday between (.*) and (.*)")]
        public void GivenTodayIsSundayBetweenAnd(string time1, string time2)
        {
            var avg = GetAverage(time1, time2);

            var weekDate = LocalDateTime.LastDayOfWeek(false).Add(TimeSpan.Parse(time1)).AddHours(avg.Hours);
            weekDate.DayOfWeek.ShouldBe(DayOfWeek.Sunday);

            var lastDayStartTime = weekDate.LastDayOfWeek(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = weekDate.LastDayOfWeek(false).Add(TimeSpan.Parse(time2));
            weekDate.ShouldBeInRange(lastDayStartTime, lastDayEndTime);

            SetCurrentDateTimeOffset(weekDate);
        }

        [Given(@"today is not sunday between (.*) and (.*)")]
        [When(@"today is not sunday between (.*) and (.*)")]
        [Then(@"today is not sunday between (.*) and (.*)")]
        public void GivenTodayIsNotSundayBetweenAnd(string time1, string time2)
        {
            var weekDate = LocalDateTime;
            if (weekDate.DayOfWeek == DayOfWeek.Sunday)
            {
                weekDate = weekDate.AddDays(1);
            }

            weekDate.DayOfWeek.ShouldNotBe(DayOfWeek.Sunday);

            var lastDayStartTime = weekDate.Add(TimeSpan.Parse(time1));
            var lastDayEndTime = weekDate.Add(TimeSpan.Parse(time2));
            weekDate.ShouldNotBeInRange(lastDayStartTime, lastDayEndTime);
            SetCurrentDateTimeOffset(weekDate);
        }

        private static TimeSpan GetAverage(string time1, string time2)
        {
            var diff = TimeSpan.Parse(time2) - TimeSpan.Parse(time1);
            var avg = diff / 2;

            return avg;
        }

        [Given(@"for report purpose, today is the last day of the month between (.*) and (.*)")]
        [When(@"for report purpose, today is the last day of the month between (.*) and (.*)")]
        [Then(@"for report purpose, today is the last day of the month between (.*) and (.*)")]
        public void ForReportTodayIsTheLastDayOfTheMonthBetweenAnd(string time1, string time2)
        {
            var avg = GetAverage(time1, time2);

            var monthDate = LocalDateTime.LastDayOfMonth(false).Add(TimeSpan.Parse(time1)).AddHours(avg.Hours);

            var lastDayStartTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time2));

            monthDate.ShouldBeInRange(lastDayStartTime, lastDayEndTime);
            SetCurrentDateTimeOffset(monthDate);
        }

        [Given(@"for report purpose, today is the last day of the month and sunday between (.*) and (.*)")]
        [When(@"for report purpose, today is the last day of the month and sunday between (.*) and (.*)")]
        [Then(@"for report purpose, today is the last day of the month and sunday between (.*) and (.*)")]
        public void ForReportTodayIsTheLastDayOfTheMonthAndSundayBetweenAnd(string time1, string time2)
        {
            var avg = GetAverage(time1, time2);

            //Mock datetime to be sunday and last date of month
            var dateTime = new DateTime(2020, 5, 31);
            var dateTimeOffset = new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime));
            var monthDate = dateTimeOffset.LastDayOfMonth(false).Add(TimeSpan.Parse(time1)).AddHours(avg.Hours);

            var lastDayStartTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time2));

            monthDate.ShouldBeInRange(lastDayStartTime, lastDayEndTime);
            monthDate.DayOfWeek.ShouldBe(DayOfWeek.Sunday);
            SetCurrentDateTimeOffset(monthDate);
        }

        [Given(@"for report purpose, today is not the last day of the month between (.*) and (.*)")]
        [When(@"for report purpose, today is not the last day of the month between (.*) and (.*)")]
        [Then(@"for report purpose, today is not the last day of the month between (.*) and (.*)")]
        public void ForReportTodayIsNotTheLastDayOfTheMonthBetweenAndString(string time1, string time2)
        {

            var monthDate = LocalDateTime;

            if (monthDate.Day == monthDate.LastDayOfMonth(false).Day)
            {
                monthDate = monthDate.AddDays(1);
            }

            var lastDayStartTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = monthDate.LastDayOfMonth(false).Add(TimeSpan.Parse(time2));

            monthDate.ShouldNotBeInRange(lastDayStartTime, lastDayEndTime);
            SetCurrentDateTimeOffset(monthDate);
        }

        [Given(@"for report purpose, today is sunday between (.*) and (.*)")]
        [When(@"for report purpose, today is sunday between (.*) and (.*)")]
        [Then(@"for report purpose, today is sunday between (.*) and (.*)")]
        public void ForReportTodayIsSundayBetweenAnd(string time1, string time2)
        {
            var avg = GetAverage(time1, time2);

            var weekDay = LocalDateTime.LastDayOfWeek(false).Add(TimeSpan.Parse(time1)).AddHours(avg.Hours);
            weekDay.DayOfWeek.ShouldBe(DayOfWeek.Sunday);

            var lastDayStartTime = weekDay.LastDayOfWeek(false).Add(TimeSpan.Parse(time1));
            var lastDayEndTime = weekDay.LastDayOfWeek(false).Add(TimeSpan.Parse(time2));
            weekDay.ShouldBeInRange(lastDayStartTime, lastDayEndTime);

            SetCurrentDateTimeOffset(weekDay);
        }

        [Given(@"for report purpose, today is not sunday between (.*) and (.*)")]
        [When(@"for report purpose, today is not sunday between (.*) and (.*)")]
        [Then(@"for report purpose, today is not sunday between (.*) and (.*)")]
        public void ForReportTodayIsNotSundayBetweenAnd(string time1, string time2)
        {
            var weekDay = LocalDateTime;
            if (weekDay.DayOfWeek == DayOfWeek.Sunday)
            {
                weekDay = weekDay.AddDays(1);
            }

            weekDay.DayOfWeek.ShouldNotBe(DayOfWeek.Sunday);

            var lastDayStartTime = weekDay.Add(TimeSpan.Parse(time1));
            var lastDayEndTime = weekDay.Add(TimeSpan.Parse(time2));
            weekDay.ShouldNotBeInRange(lastDayStartTime, lastDayEndTime);

            SetCurrentDateTimeOffset(weekDay);
        }


        [When(@"today is the first day of last week")]
        public void WhenTodayIsTheFirstDayOfLastWeek()
        {
            var weekDate = DateTime.Now.AddDays(-7).FirstDayOfWeek(false);
            if (DateTime.Now.Day == DateTime.Now.LastDayOfWeek(false).Day)
            {
                weekDate = DateTime.Now.AddDays(-2).FirstDayOfWeek(false);
            }
            weekDate.DayOfWeek.ShouldBe(DayOfWeek.Monday);
            weekDate.ShouldBeLessThan(DateTime.Now);

            SetCurrentDateTimeOffset(weekDate);
        }

        [When(@"today is the first day of this week")]
        public void WhenTodayIsTheFirstDayOfThisWeek()
        {
            var weekDate = DateTime.Now.FirstDayOfWeek(false);
            weekDate.DayOfWeek.ShouldBe(DayOfWeek.Monday);
            SetCurrentDateTimeOffset(weekDate);
        }

        [When(@"today is the first day of last month")]
        public void WhenTodayIsTheFirstDayOfLastMonth()
        {
            var monthDate = DateTime.Now.AddMonths(-1).FirstDayOfMonth(false);
            monthDate.Day.ShouldBe(1);
            monthDate.ShouldBeLessThan(DateTime.Now);

            SetCurrentDateTimeOffset(monthDate);
        }

        [When(@"today is the first day of this month")]
        public void WhenTodayIsTheFirstDayOfThisMonth()
        {
            var monthDate = DateTime.Now.FirstDayOfMonth(false);
            monthDate.Day.ShouldBe(1);
            SetCurrentDateTimeOffset(monthDate);
        }

        #endregion

        [When(@"I reload the quizzes")]
        [Then(@"I reload the quizzes")]
        public void WhenIReloadTheQuizzes()
        {
            int? lastCount = null;
            
            // deterministic cycling through the empty refresh (0), app launch refresh, and test fired refresh

            _viewModel.GetAvailableUserQuizzesCommand?.WaitHandle.WaitOne();

            var result = RetryHelpers.Retry(() =>
            {
                _viewModel.GetAvailableUserQuizzesCommand.Execute();
                _viewModel.GetAvailableUserQuizzesCommand?.WaitHandle.WaitOne();
                
                var currentCount = _viewModel.UserQuizzes.Count();

                if (lastCount == currentCount)
                    return true;

                lastCount = currentCount;

                return false;

            }, 200);

            result.ShouldBeTrue();
        }

        private void SetCurrentDateTimeOffset(DateTimeOffset currentDateTime)
        {
            LocalDateTime = currentDateTime;
            ((DateTimeOffsetProvider)CacheServices.QuizCacheService.DateTimeOffsetProvider).SetCurrentDateTimeOffset(currentDateTime);
        }
    }
}
