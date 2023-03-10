using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using SwiftCaps.Admin.Services.Quiz.Extensions;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Helpers;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.Core.Common.Helpers;
using DayOfWeek = System.DayOfWeek;
using Schedule = SwiftCaps.Models.Models.Schedule;
using User = SwiftCaps.Models.Models.User;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services
{
    public class AdminReportingService : IAdminReportingService
    {
        private readonly SwiftCapsContext _dbContext;

        public AdminReportingService(SwiftCapsContext swiftCapsContext)
        {
            _dbContext = swiftCapsContext;
        }
        public async Task<IList<LeaderBoard>> GetLeaderboardAsync(AdminReportingRequest request)
        {
            Guard.Against.InvalidReportingLeaderboardPayload(request);

            var groupExists = await _dbContext.Groups.AnyAsync(g => g.Id == request.GroupId);
            if (!groupExists)
            {
                throw new NotFoundException(request.GroupId.ToString(), "Group");
            }

            // get users from same group
            var usersList = await _dbContext.Users.Where((x => x.GroupId == request.GroupId))
                                                  .ToListAsync();

            var userLocalDateTime = request.ClientLocalDateTime;

            // check if is time to add a new monthly test to list
            var monthShift = userLocalDateTime.Day == userLocalDateTime.DateTime.LastDayOfMonth(false).Day &&
                             userLocalDateTime.TimeOfDay >= TimeshiftConstants.TIME_SHIFT_START_TIME &&
                             userLocalDateTime.TimeOfDay <= TimeshiftConstants.TIME_SHIFT_END_TIME;

            // check if is time to add a new weekly test to list
            var weekShift = userLocalDateTime.Day == userLocalDateTime.DateTime.LastDayOfWeek(false).Day &&
                            userLocalDateTime.TimeOfDay >= TimeshiftConstants.TIME_SHIFT_START_TIME &&
                            userLocalDateTime.TimeOfDay <= TimeshiftConstants.TIME_SHIFT_END_TIME;

            var leaderBoards = await GenerateLeaderBoards(usersList, userLocalDateTime, weekShift, monthShift);
            return leaderBoards;
        }

        public async Task<IList<GroupProgressReportItem>> GetGroupProgressReport(AdminReportingRequest request)
        {
            var reportItems = new List<GroupProgressReportItem>();
            try
            {
                var monthShift = CalculateMonthShift(request.ClientLocalDateTime);
                var monthExpiry = QuizHelper.GetMonthlyExpiry(request.ClientLocalDateTime, monthShift);

                var weekShift = CalculateWeekShift(request.ClientLocalDateTime);
                var weekExpiry = QuizHelper.GetWeeklyExpiry(request.ClientLocalDateTime, weekShift);

                var groups = await GetGroupsWithUsers();

                foreach (var group in groups)
                {
                    var groupUsers = group.Users;
                    var groupUsersIds = groupUsers.Select(u => u.Id);

                    var item = new GroupProgressReportItem
                    {
                        GroupName = group.Name,
                        UserCount = groupUsers.Count
                    };
                    var schedules = await GetSchedulesByGroup(group);
                    foreach (var schedule in schedules)
                    {
                        FillQuizMetadata(monthExpiry, weekExpiry, item, schedule);
                        var currentGroupUserQuizzes = schedule.UserQuizzes.Where(uq => groupUsersIds.Contains(uq.UserId));
                        FillQuizStatistics(item, currentGroupUserQuizzes);
                    }
                    reportItems.Add(item);
                }
                return reportItems;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IList<GroupAverageReportItem>> GetGroupAverageReport()
        {
            var reportItems = new List<GroupAverageReportItem>();
            try
            {
                var groups = await GetGroupsWithUsers();

                foreach (var group in groups)
                {
                    var userCount = group.Users.Count;
                    var schedules = await GetSchedulesByGroup(group);
                    var item = new GroupAverageReportItem
                    {
                        GroupName = group.Name,
                        UserCount = userCount,
                        QuizCount = schedules.Select(s => s.Quiz.Id).ToList().Distinct().Count(),
                    };
                    
                    var userQuizzes = schedules.SelectMany(s => s.UserQuizzes).ToList();
                    if(userCount > 0)
                    {
                        var averageDone = 100 * userQuizzes.Sum(uq => uq.Completed.HasValue ? 1 : 0) / userCount;
                        item.AverageDonePercentage = averageDone;
                    }
                    if(userQuizzes.Count() > 0)
                    {
                        var avgTime = new TimeSpan(userQuizzes.Sum(uq => uq.Completed.Value.Ticks - uq.Created.Ticks) / userQuizzes.Count());
                        item.AvergageTime = $"{avgTime.Minutes}m {avgTime.Seconds}s";
                    }
                    reportItems.Add(item);
                }
                return reportItems;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IList<QuizAverageReportItem>> GetQuizAverageReport()
        {
            var reportItems = new List<QuizAverageReportItem>();
            try
            {
                var quizzes = await _dbContext.Quizzes
                                              .OrderBy(q => q.Name)  
                                              .Select(q => new SCModels.Quiz{
                                                  Id = q.Id,
                                                  Name = q.Name
                                              })
                                              .ToListAsync();

                foreach (var quiz in quizzes)
                {
                    var item = new QuizAverageReportItem
                    {
                        QuizName = quiz.Name
                    };

                    var schedules = await _dbContext.Schedules
                                                    .Include(s => s.ScheduleGroups)
                                                        .ThenInclude(sg => sg.Group)
                                                            .ThenInclude(g => g.Users)
                                                    .Include(s => s.UserQuizzes)
                                                    .Where(s => s.QuizId == quiz.Id)
                                                    .ToListAsync();
                    var groupsCount = schedules.SelectMany(s => 
                                            s.ScheduleGroups.Select(sg => sg.Group.Id)
                                        )
                                        .Distinct()
                                        .Count();
                    item.GroupCount = groupsCount;

                    var userCount = schedules.SelectMany(s => 
                                            s.ScheduleGroups.SelectMany(sg => sg.Group.Users)
                                        )
                                        .Select(u => u.Id)
                                        .Distinct()
                                        .Count();
                    item.UserCount = userCount;
                    if(userCount > 0)
                    {
                        var doneCount = schedules.SelectMany(s => s.UserQuizzes)
                                                .Sum(uq => uq.Completed.HasValue ? 1 : 0);
                        var donePercentage = 100 * doneCount/userCount;
                        item.DonePercentage = donePercentage;
                    }
                    reportItems.Add(item);
                }
                return reportItems;
            }
            catch
            {
                throw;
            }
        }

        
        private static void FillQuizStatistics(GroupProgressReportItem item, IEnumerable<UserQuiz> scheduleUserQuizzes)
        {
            if (scheduleUserQuizzes?.Count() > 0)
            {
                var avgTime = new TimeSpan(scheduleUserQuizzes.Sum(uq => uq.Completed.Value.Ticks - uq.Created.Ticks) / scheduleUserQuizzes.Count());
                item.AvergageTime = $"{avgTime.Minutes}m {avgTime.Seconds}s";
                if (item.UserCount > 0)
                {
                    var percentDone = 100 * scheduleUserQuizzes.Sum(uq => uq.Completed.HasValue ? 1 : 0) / item.UserCount;
                    item.DonePercentage = percentDone;
                }
            }
        }

        private static void FillQuizMetadata(DateTimeOffset monthExpiry, DateTimeOffset weekExpiry, GroupProgressReportItem item, Schedule schedule)
        {
            item.QuizName = schedule.Quiz.Name;
            item.Recurrence = schedule.Recurrence;
            item.Sequence = schedule.Recurrence == Recurrence.Monthly
                            ? monthExpiry.ToString("MMMM") :
                            $"Week {CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(weekExpiry.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}";
        }

        private async Task<List<Schedule>> GetSchedulesByGroup(Group group)
        {
            return await _dbContext.ScheduleGroups
                                                                .Include(sg => sg.Schedule)
                                                                    .ThenInclude(s => s.Quiz)
                                                                .Include(sg => sg.Schedule)
                                                                    .ThenInclude(s => s.UserQuizzes)
                                                                .Where(sg => sg.GroupId == group.Id)
                                                                .Select(sg => new Schedule
                                                                {
                                                                    Id = sg.Schedule.Id,
                                                                    Quiz = new Models.Models.Quiz
                                                                    {
                                                                        Id = sg.Schedule.Quiz.Id,
                                                                        Name = sg.Schedule.Quiz.Name
                                                                    },
                                                                    Recurrence = sg.Schedule.Recurrence,
                                                                    UserQuizzes = sg.Schedule.UserQuizzes
                                                                })
                                                                .ToListAsync();
        }

        private async Task<List<Group>> GetGroupsWithUsers()
        {
            return await _dbContext.Groups
                                   .Include(g => g.Users)
                                   .OrderBy(g => g.Name)
                                   .ToListAsync();
        }

        private async Task<List<LeaderBoard>> GenerateLeaderBoards(IList<User> users, DateTimeOffset clientLocalDateTime, bool weekShift, bool monthShift)
        {
            var leaderBoards = new List<LeaderBoard>();
            // for each user from user group
            foreach (var user in users)
            {
                //create object to add to returnable list
                var leaderBoardRow = new LeaderBoard
                {
                    UserId = user.Id,
                    UserName = user.FullName,
                    MonthlyQuizReports = new List<QuizReport>(),
                    WeeklyQuizReports = new List<QuizReport>()
                };

                var userQuizzes = await _dbContext.UserQuizzes
                                                  .Include(u => u.Schedule)
                                                  .Where(x => x.UserId == user.Id &&
                                                        (x.Expiry > clientLocalDateTime.DateTime))?
                                                  .ToListAsync();

                // no user quiz found, add empty user quiz to report
                if (userQuizzes?.Count == 0)
                {
                    leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                    leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());

                    if (weekShift)
                    {
                        leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                    }
                    if (monthShift)
                    {
                        leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());
                    }
                }
                else
                {
                    foreach (var userQuiz in userQuizzes)
                    {
                        //Schedule of that user's each group quiz
                        //var schedule = await _dbContext.Schedules.FindAsync(userQuiz.ScheduleId);

                        if (userQuiz.Schedule.Recurrence == Recurrence.Weekly)
                        {
                            leaderBoardRow.WeeklyQuizReports.Add(new QuizReport
                            {
                                Recurrence = userQuiz.Schedule.Recurrence,
                                IsCompleted = userQuiz.Completed.HasValue,
                                QuizId = userQuiz.Schedule.QuizId,
                                ExpiryDate = userQuiz.Expiry
                            });
                            //Because timeshift if only 1 quiz add 1 more empty quiz report
                            if (weekShift && userQuizzes.Count(x => x.Schedule?.Recurrence == Recurrence.Weekly) == 1)
                            {
                                leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                            }
                        }
                        else
                        {
                            leaderBoardRow.MonthlyQuizReports.Add(new QuizReport
                            {
                                Recurrence = userQuiz.Schedule.Recurrence,
                                IsCompleted = userQuiz.Completed.HasValue,
                                QuizId = userQuiz.Schedule.QuizId,
                                ExpiryDate = userQuiz.Expiry
                            });
                            //Because timeshift if only 1 quiz add 1 more empty quiz report
                            if (monthShift && userQuizzes.Count(x => x.Schedule?.Recurrence == Recurrence.Monthly) == 1)
                            {
                                leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());
                            }
                        }
                    }

                    if (!leaderBoardRow.WeeklyQuizReports.Any())
                    {
                        // add an empty weekly report for the row
                        leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());

                        if (weekShift)
                        {
                            leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                        }
                    }

                    if (!leaderBoardRow.MonthlyQuizReports.Any())
                    {
                        // add an empty weekly report for the row
                        leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());

                        if (monthShift)
                        {
                            leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());
                        }
                    }
                }

                leaderBoards.Add(leaderBoardRow);
            }
            return leaderBoards.OrderBy(l => l.UserName).ToList();
        }

        private static bool CalculateWeekShift(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.Day == dateTimeOffset.DateTime.LastDayOfWeek(false).Day &&
                   dateTimeOffset.TimeOfDay >= TimeshiftConstants.TIME_SHIFT_START_TIME &&
                   dateTimeOffset.TimeOfDay <= TimeshiftConstants.TIME_SHIFT_END_TIME;
        }

        private static bool CalculateMonthShift(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.Day == dateTimeOffset.DateTime.LastDayOfMonth(false).Day &&
                   dateTimeOffset.TimeOfDay >= TimeshiftConstants.TIME_SHIFT_START_TIME &&
                   dateTimeOffset.TimeOfDay <= TimeshiftConstants.TIME_SHIFT_END_TIME;
        }
    }
}
