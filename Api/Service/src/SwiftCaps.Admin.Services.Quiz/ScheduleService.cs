using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using SwiftCaps.Admin.Services.Quiz.Extensions;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly SwiftCapsContext _context;

        public ScheduleService(SwiftCapsContext context)
        {
            _context = context;
        }

        public async Task<IList<ScheduleSummary>> GetSchedulesAsync()
        {
            var query = _context.Schedules
                                .Include(schedule => schedule.Quiz)
                                .Include(schedule => schedule.ScheduleGroups)
                                .OrderBy(schedule => schedule.Quiz.Name)
                                .Select(schedule => new ScheduleSummary
                                {
                                    Id = schedule.Id,
                                    QuizName = schedule.Quiz.Name,
                                    Recurrence = schedule.Recurrence,
                                    Start = schedule.StartTime,
                                    End = schedule.EndTime,
                                    GroupCount = schedule.ScheduleGroups.Count,
                                    Created = schedule.Created,
                                    Updated = schedule.Updated
                                });
            return await query.ToListAsync(); ;
        }

        public async Task<Schedule> GetScheduleAsync(Guid scheduleId)
        {
            Guard.Against.InvalidScheduleReadPayload(scheduleId);
            var schedule = await _context.Schedules
                                .Include(schedule => schedule.Quiz)
                                .Select(schedule => new Schedule
                                {
                                    Id = schedule.Id,
                                    QuizId = schedule.Quiz.Id,
                                    Quiz = new SCModels.Quiz
                                    {
                                        Id = schedule.Quiz.Id,
                                        Name = schedule.Quiz.Name
                                    },
                                    StartTime = schedule.StartTime,
                                    EndTime=schedule.EndTime,
                                    Recurrence = schedule.Recurrence,
                                    Updated = schedule.Updated,
                                    Created= schedule.Created
                                })
                                .SingleOrDefaultAsync(s => s.Id == scheduleId);
                                    
            if (schedule == null)
            {
                throw new NotFoundException(scheduleId.ToString(), "Schedule");
            }

            var scheduleGroups = await _context.ScheduleGroups
                                               .Where(sg => sg.ScheduleId == scheduleId)
                                               .Include(sg => sg.Group)
                                               .Select(sg => new ScheduleGroup{
                                                    Id = sg.Id,
                                                    ScheduleId= sg.ScheduleId,
                                                    Created= sg.Created,
                                                    Updated= sg.Updated,
                                                    GroupId = sg.GroupId,
                                                    Group = new Group
                                                    { 
                                                        Id = sg.Group.Id , 
                                                        Name = sg.Group.Name 
                                                    }
                                                })
                                               .ToListAsync();
            schedule.ScheduleGroups = scheduleGroups;

            return schedule;
        }

        public async Task<Guid?> CreateScheduleAsync(Schedule newSchedule)
        {
            Guard.Against.InvalidScheduleCreatePayload(newSchedule);

            try
            {
                var quiz = await _context.Quizzes.FindAsync(newSchedule.QuizId);
                if (quiz == null)
                {
                    throw new NotFoundException(newSchedule.QuizId.ToString(), "Quiz");
                }
                var scheduleToCreate = new Schedule()
                {
                    Id = Guid.NewGuid(),
                    QuizId = newSchedule.QuizId,
                    Recurrence = newSchedule.Recurrence,
                    StartTime = newSchedule.StartTime.Value.ToUniversalTime(),
                    EndTime = newSchedule.EndTime.Value.ToUniversalTime(),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                };
                var resultAdd = await _context.Schedules.AddAsync(scheduleToCreate);
                await _context.SaveChangesAsync();
                return resultAdd.Entity.Id;
            }
            catch 
            {
                throw;
            }
        }
        public async Task<Guid?> UpdateScheduleAsync(Guid scheduleId, Schedule updatedSchedule)
        {
            Guard.Against.InvalidScheduleUpdatePayload(scheduleId, updatedSchedule);
            if(updatedSchedule.EndTime.Value.ToUniversalTime().Date < updatedSchedule.StartTime.Value.ToUniversalTime().Date)
            { 
                throw new InvalidOperationException("End time cannot be less than Start time.");
            }

            try
            {
                var scheduleToUpdate = await _context.Schedules
                                                     .Include(s => s.ScheduleGroups)
                                                     .SingleOrDefaultAsync(s => s.Id == scheduleId);
                if (scheduleToUpdate == null)
                {
                    throw new NotFoundException(scheduleId.ToString(), "Schedule");
                }

                var scheduleActive = scheduleToUpdate.StartTime.Value.ToUniversalTime().Date <= DateTime.UtcNow.Date &&
                                     scheduleToUpdate.EndTime.Value.ToUniversalTime().Date >= DateTime.UtcNow.Date;
                var scheduleAssigned = scheduleToUpdate.ScheduleGroups.Count > 0;
                var quizChanged = !scheduleToUpdate.QuizId.Equals(updatedSchedule.QuizId);
                var startChanged = scheduleToUpdate.StartTime.Value.ToUniversalTime().Date != updatedSchedule.StartTime.Value.ToUniversalTime().Date;
                var recurrenceChanged = !(scheduleToUpdate.Recurrence == updatedSchedule.Recurrence);
                var particularsChanged = quizChanged || startChanged || recurrenceChanged;
                if (scheduleActive && scheduleAssigned && particularsChanged)
                {
                    throw new InvalidOperationException("Cannot modify schedule when it is active.");
                }
                scheduleToUpdate.QuizId = updatedSchedule.QuizId;
                scheduleToUpdate.StartTime = updatedSchedule.StartTime.Value.ToUniversalTime().Date;
                scheduleToUpdate.Recurrence = updatedSchedule.Recurrence;
                scheduleToUpdate.EndTime = updatedSchedule.EndTime.Value.ToUniversalTime().Date;
                scheduleToUpdate.Updated = DateTime.UtcNow;
                var resultAdd = _context.Update(scheduleToUpdate);
                await _context.SaveChangesAsync();
                return resultAdd.Entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteScheduleAsync(Guid scheduleId)
        {
            Guard.Against.InvalidScheduleDeletePayload(scheduleId);
            try
            {
                var scheduleToDelete = await _context.Schedules
                                                     .Include(s => s.ScheduleGroups)
                                                     .SingleOrDefaultAsync(s => s.Id == scheduleId);
                if (scheduleToDelete == null)
                {
                    throw new NotFoundException(scheduleId.ToString(), "Schedule");
                }

                var scheduleActive = scheduleToDelete.StartTime.Value.ToUniversalTime().Date <= DateTime.UtcNow.Date
                                     && scheduleToDelete.EndTime.Value.ToUniversalTime().Date >= DateTime.UtcNow.Date;
                var scheduleAssigned = scheduleToDelete.ScheduleGroups.Count > 0;
                if (scheduleActive && scheduleAssigned)
                {
                    throw new InvalidOperationException("Schedule is currently active. Cannot delete Schedule");
                }
                _context.Remove(scheduleToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

    }
}
