using System;
using System.Threading.Tasks;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IGroupService
    {
        Task<ServiceResponse<Guid>> GetOrCreateGroup(Guid groupId, string groupName);
    }
}
