using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IScheduleService
    {
        Task<IList<ScheduleSummary>> GetSchedulesAsync();
        Task<Schedule> GetScheduleAsync(Guid scheduleId);
        Task<Guid?> CreateScheduleAsync(Schedule newSchedule);
        Task<Guid?> UpdateScheduleAsync(Guid scheduleId, Schedule updatedSchedule);
        Task<bool> DeleteScheduleAsync(Guid scheduleId);
    }
}
