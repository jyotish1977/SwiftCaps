using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
using Refit;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Shared.Clients
{
    public interface IReportingClient
    {
        [Post("/admin/reporting/leaderboard/groups/{groupId}")]
        Task<ServiceResponse<IList<LeaderBoard>>> GetLeaderBoard(Guid groupId, AdminReportingRequest model);


        [Post("/admin/reporting/groupprogress")]
        Task<ServiceResponse<IList<GroupProgressReportItem>>> GetGroupPogressReport(AdminReportingRequest model);

        [Get("/admin/reporting/groupaverage")]
        Task<ServiceResponse<IList<GroupAverageReportItem>>> GetGroupAverageReport();

        [Get("/admin/reporting/quizaverage")]
        Task<ServiceResponse<IList<QuizAverageReportItem>>> GetQuizAverageReport();
    }
}
