using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Shared.Clients
{
    public interface IScheduleClient
    {
        [Get("/admin/schedules")]
        Task<ServiceResponse<IList<ScheduleSummary>>> GetSchedules();

        [Get("/admin/schedules/{scheduleId}")]
        Task<ServiceResponse<Schedule>> GetSchedule(Guid scheduleId);

        [Post("/admin/schedules")]
        Task<ServiceResponse<Guid?>> AddSchedule(Schedule schedule);

        [Put("/admin/schedules/{scheduleId}")]
        Task<ServiceResponse<Guid?>> UpdateSchedule(Guid scheduleId, Schedule schedule);

        [Delete("/admin/schedules/{scheduleId}")]
        Task<HttpResponseMessage> DeleteSchedule(Guid scheduleId);
    }
}
