using System;
using System.Threading.Tasks;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<Models.Models.User>> GetOrCreateUser(Guid userId, string accessToken = null);
    }
}
