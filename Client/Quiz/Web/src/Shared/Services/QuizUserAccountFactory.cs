using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using SwiftCaps.Client.Shared.Models;
using SwiftCAPS.Web.Shared.Clients;

namespace SwiftCAPS.Web.Shared.Services
{
    public class QuizUserAccountFactory : AccountClaimsPrincipalFactory<SwiftCapsUser>
    {
        private readonly HttpClient _httpClient;
        public QuizUserAccountFactory(IAccessTokenProviderAccessor accessor, HttpClient httpClient) : base(accessor)
        {
            _httpClient = httpClient;
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(SwiftCapsUser account, RemoteAuthenticationUserOptions options)
        {
            var initialUser = await base.CreateUserAsync(account, options);

            if (initialUser.Identity.IsAuthenticated)
            {
                var result = await TokenProvider.RequestAccessToken();
                AccessToken accessToken;
                var isValidToken = result.TryGetToken(out accessToken);

                if (isValidToken)
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, "/api/user/getorcreateuser");
                    message.Headers.Add("Authorization", $"Bearer {accessToken.Value}");

                    var responseMessage = await _httpClient.SendAsync(message);
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        var claimsIdentity = new ClaimsIdentity();
                        claimsIdentity.AddClaims(initialUser.Claims);
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, nameof(RoleConstants.User)));
                        initialUser.AddIdentity(claimsIdentity);
                    }
                }
            }

            return initialUser;
        }
    }
}
