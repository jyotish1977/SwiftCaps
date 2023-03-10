using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IScheduleGroupService
    {
        Task<List<ScheduleGroupSummary>> GetGroupsAsync(Guid scheduleId);
        Task<Guid?> CreateGroupAsync(Guid scheduleId, Group group);
        Task<bool> DeleteGroupAsync(Guid scheduleId, Guid groupId);
        Task<Group[]> SearchGroupsAsync(Guid scheduleId, string searchString);

    }

    
}
