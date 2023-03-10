using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IAdminReportingService
    {
        public Task<IList<LeaderBoard>> GetLeaderboardAsync(AdminReportingRequest adminLeaderboardRequest);
        public Task<IList<GroupProgressReportItem>> GetGroupProgressReport(AdminReportingRequest request);
        public Task<IList<GroupAverageReportItem>> GetGroupAverageReport();
        public Task<IList<QuizAverageReportItem>> GetQuizAverageReport();
    }
}
