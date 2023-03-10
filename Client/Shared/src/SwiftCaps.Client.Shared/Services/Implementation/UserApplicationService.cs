using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SwiftCaps.Client.Shared.Services.Abstraction;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Client.Shared.Services.Implementation
{
    public class UserApplicationService : IUserApplicationService
    {
        private readonly IReadOnlyList<Application> _applications;

        public UserApplicationService(IConfiguration configuration)
        {
            var applicationsSection = configuration.GetSection("Applications");
            _applications = applicationsSection.Get<IReadOnlyList<Application>>();
        }

        public Task<IReadOnlyList<Application>> GetApplicationsAsync()
        {
            return Task.FromResult(_applications);
        }
    }
}
