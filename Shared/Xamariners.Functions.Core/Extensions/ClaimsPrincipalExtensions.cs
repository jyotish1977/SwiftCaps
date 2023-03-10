using System;
using System.Linq;
using System.Security.Claims;

namespace Xamariners.Functions.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasScope(this ClaimsPrincipal principal, string scope)
        {
            var claimScope = principal?.Claims?.FirstOrDefault(c => c?.Type == "scp")?.Value;
            return claimScope != null && claimScope.Contains(scope, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EnsureScope(this ClaimsPrincipal principal, string scope)
        {
            var claimScope = principal?.Claims?.FirstOrDefault(c => c?.Type == "scp")?.Value;
            if(claimScope == null || !claimScope.Contains(scope, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedAccessException("Access denied.");
            };
            return true;
        }
    }
}
