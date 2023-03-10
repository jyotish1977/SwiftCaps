using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Client.Shared.Services.Abstraction
{
    public interface IUserApplicationService
    {
        Task<IReadOnlyList<Application>> GetApplicationsAsync();
    }
}
