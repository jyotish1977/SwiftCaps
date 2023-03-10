using System;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IGraphService
    {
        Task<ServiceResponse<GraphUser>> GetUser(string accessToken, Guid userId);
    }
}
