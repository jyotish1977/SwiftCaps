using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Extensions;
using Xamariners.Functions.Core.Interfaces;

namespace Xamariners.Functions.Core.Infrastructure
{
    public abstract class FunctionBase
    {
        protected ILogger Logger;
        private readonly AzureADConfiguration _azureAdConfiguration;
        private readonly IFunctionAuthorizationService _functionAuthorization;
        protected string AccessToken;

        protected FunctionBase(AzureADConfiguration azureAdConfiguration, IFunctionAuthorizationService functionAuthorization)
        {
            _azureAdConfiguration = azureAdConfiguration;
            _functionAuthorization = functionAuthorization;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            
        }

        protected async Task<Guid> Authorize(AuthenticationHeaderValue authenticationHeader)
        {
            var audience = _azureAdConfiguration.Audience;
            var clientsAllowed = _azureAdConfiguration.Clients;
            var tenantId = _azureAdConfiguration.TenantId;
            var userId = await _functionAuthorization.Authorize(authenticationHeader, 
                tenantId, 
                audience, 
                clientsAllowed);
            return userId;
        }

        protected ClaimsPrincipal ValidateToken(AuthenticationHeaderValue authenticationHeaderValue)
        {
            var token = authenticationHeaderValue?.Parameter;
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Access denied.");
            }
            AccessToken = token;
            var audience = _azureAdConfiguration.Audience;
            var clientsAllowed = _azureAdConfiguration.Clients;
            var tenantId = _azureAdConfiguration.TenantId;
            var principal = _functionAuthorization.ValidateToken(token, tenantId, audience, clientsAllowed);
            if (principal == null)
            {
                throw new UnauthorizedAccessException("Access denied.");
            }
            return principal;
        }

        protected void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogException(e.ExceptionObject as Exception);
        }

        protected static T ParseClaim<T>(ClaimsPrincipal principal,string claim)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T) converter.ConvertFrom(principal?.Claims?.FirstOrDefault(c => c?.Type == claim)?.Value);
        }

        protected static async Task<T> GetRequestContent<T>(Stream reqBody) where T : class
        {
            var json = await new StreamReader(reqBody).ReadToEndAsync();
            var content = JsonConvert.DeserializeObject<T>(json);
            return content;
        }

        protected ClaimsPrincipal EnsureAuthorization(HttpRequestData request, string scope)
        {
            request.EnsureAuthorizationHeader();
            var authHeaderValue = request.GetAuthenticationHeaderValue();
            var userPrincipal = ValidateToken(authHeaderValue);
            if(!string.IsNullOrWhiteSpace(scope))
            {
                userPrincipal.EnsureScope(scope);
            }
            return userPrincipal;
        }
    }
}
