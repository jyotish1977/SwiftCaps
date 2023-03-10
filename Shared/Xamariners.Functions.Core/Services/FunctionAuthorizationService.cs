using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Xamariners.Functions.Core.Interfaces;

namespace Xamariners.Functions.Core.Services
{
    public class FunctionAuthorizationService : IFunctionAuthorizationService
    {
        private const string issuerDomain = "https://sts.windows.net";
        
        public async Task<Guid> Authorize(AuthenticationHeaderValue authenticationHeader, string tenantId, string audience, string clientsAllowed)
        {
            var bearerToken = authenticationHeader?.Parameter;
            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new UnauthorizedAccessException("Access Denied");
            }
            try
            {
                var principal = ValidateToken(bearerToken, tenantId, audience, clientsAllowed);
                if (principal == null)
                {
                    return Guid.Empty;
                }
                return await GetUserId(principal);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public Task<Guid> GetUserId(ClaimsPrincipal principal)
        {
            try
            {
                return Task.FromResult(Guid.Parse(principal.Identity.Name));
            }
            catch (Exception)
            {
                return Task.FromResult(Guid.Empty);
            }
        }

        public ClaimsPrincipal ValidateToken(string jwtToken, string tenantId, string audience, string clientsAllowed)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(jwtToken))
            {
                return null;
            }
            handler.InboundClaimTypeMap.Clear();
            ClaimsPrincipal principal = null;
            try
            {
                principal = handler.ValidateToken(jwtToken, new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudiences = new string[] { audience },
                    ValidIssuer = $"{issuerDomain}/{tenantId}/",
                    ValidateIssuerSigningKey = false,
                    SignatureValidator = (t, param) => new JwtSecurityToken(t),
                    NameClaimType = "oid"
                }, out SecurityToken token);
                var appIdClaim = principal.Claims.FirstOrDefault(item => item.Type.Equals("appid"));
                if (!clientsAllowed.Contains(appIdClaim?.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return principal;
        }
    }
}
