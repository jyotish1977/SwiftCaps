using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Xamariners.Functions.Core.Interfaces
{
    public interface IFunctionAuthorizationService
    {
        Task<Guid> Authorize(AuthenticationHeaderValue authenticationHeader, string tenantId, string audience, string clientsAllowed);
        Task<Guid> GetUserId(ClaimsPrincipal principal);
        ClaimsPrincipal ValidateToken(string jwtToken, string tenantId, string audience, string clientsAllowed);
    }
}
