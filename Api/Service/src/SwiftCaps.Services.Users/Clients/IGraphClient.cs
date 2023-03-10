using System;
using System.Threading.Tasks;
using Refit;

namespace SwiftCaps.Services.User.Clients
{
    public interface IGraphClient
    {
        [Get("/{version}/me")]
        Task<ApiResponse<string>> GetUser([Authorize("Bearer")] string token, [AliasAs("version")]string apiVersion);

        [Headers("ConsistencyLevel:eventual")]
        [Get("/{version}/me/{**route}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<ApiResponse<string>> GetGroupsAsync([Authorize("Bearer")] string token, [AliasAs("version")]string apiVersion, string route);

        [Headers("ConsistencyLevel:eventual")]
        [Get("/{version}/me/{**route}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<ApiResponse<string>> GetApplicationGroupsAsync([Authorize("Bearer")] string token, [AliasAs("version")]string apiVersion, string route);
    }
}
