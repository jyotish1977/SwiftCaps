using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Shared.Clients
{
    public interface IScheduleGroupClient
    {
        [Get("/admin/schedules/{scheduleId}/groups")]
        Task<ServiceResponse<IList<ScheduleGroupSummary>>> GetSecheduleGroups(Guid scheduleId);

        [Get("/admin/schedules/{scheduleId}/groups/search?q={search}")]
        Task<ServiceResponse<IList<Group>>> SearchGroups(Guid scheduleId, string search);
       
        [Post("/admin/schedules/{scheduleId}/groups")]
        Task<ServiceResponse<Guid?>> CreateSheduleGroup(Guid scheduleId, Group group);

        [Delete("/admin/schedules/{scheduleId}/groups/{groupId}")]
        Task<HttpResponseMessage> DeleteScheduleGroup(Guid scheduleId, Guid groupId);
    }
}
