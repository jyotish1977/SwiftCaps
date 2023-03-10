using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.User.Clients;
using Xamariners.RestClient.Helpers;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Infrastructure;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.User
{
    public class GraphService : IGraphService
    {
        private readonly string _graphApiVersion;
        private readonly IAuthenticationClient _authenticationClient;
        private readonly IGraphClient _graphClient;
        private readonly string _applicationId;
        private readonly string _applicationSecret;

        public GraphService(IConfiguration configuration, IAuthenticationClient authenticationClient,
            IGraphClient graphClient)
        {
            _graphApiVersion = configuration["GraphApiVersion"];
            _applicationId = configuration["ApplicationId"];
            _applicationSecret = configuration["ApplicationSecret"];
            _authenticationClient = authenticationClient;
            _graphClient = graphClient;
        }

        public async Task<ServiceResponse<GraphUser>> GetUser(string accessToken, Guid userId)
        {
            try
            {
                var graphToken = await GetGraphToken(accessToken);

                var graphUser = await GetUserAsync(graphToken);

                var appGroups = await GetApplicationGroupsForUserAsync(graphToken);
                if (appGroups.Count == 0)
                {
                    return new ServiceResponse<GraphUser>(ServiceStatus.Error,
                        "Error processing request",
                        "Not authorized to use SwiftCAPS application",
                        data: null)
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                }

                var graphGroups = await GetGroupsForUserAsync(graphToken);
                if (graphGroups == null || graphGroups?.Count == 0)
                {
                    return new ServiceResponse<GraphUser>(ServiceStatus.Error,
                        "Eligibility Error!",
                        "Sorry! you're not assigned to a Group, please contact the System Administrator.",
                        data: null)
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                }

                var squadronGroup = (graphGroups ?? new JArray()).FirstOrDefault();

                var user = new GraphUser
                {
                    UserId = Guid.Parse(graphUser["id"].Value<string>()),
                    FirstName = graphUser["givenName"]?.Value<string>(),
                    LastName = graphUser["surname"]?.Value<string>(),
                    FullName = graphUser["displayName"]?.Value<string>(),
                    Email = graphUser["userPrincipalName"]?.Value<string>(),
                    GroupId = Guid.Parse(squadronGroup["id"].Value<string>()),
                    GroupName = squadronGroup["displayName"]?.Value<string>(),
                };

                return user.AsServiceResponse();
            }
            catch (Exception ex)
            {
                return ServiceResponseHelpers.UnhandledServiceResponse<GraphUser>(ex);
            }
        }

        private async Task<JObject> GetUserAsync(string graphToken)
        {
            var response = await _graphClient.GetUser(graphToken, _graphApiVersion);
            await response.EnsureSuccessStatusCodeAsync();
            var graphUser = JObject.Parse(response.Content);
            return graphUser;
        }

        private async Task<JArray> GetGroupsForUserAsync(string graphToken)
        {
            var route = "memberOf/microsoft.graph.group?$select=id,displayName&$search=\"displayName:grp-sqn-\"";
            var groupResponse = await _graphClient.GetGroupsAsync(graphToken, _graphApiVersion, route);
            await groupResponse.EnsureSuccessStatusCodeAsync();
            var groupsObject = JObject.Parse(groupResponse.Content);
            var graphGroups = groupsObject["value"]?.Value<JArray>();
            return graphGroups;
        }

        private async Task<JArray> GetApplicationGroupsForUserAsync(string graphToken)
        {
            var route = "memberOf/microsoft.graph.group?$select=id,displayName&$search=\"displayName:app-ois-swiftcaps\"";
            var appGroupsResponse = await _graphClient.GetApplicationGroupsAsync(graphToken, _graphApiVersion, route);
            await appGroupsResponse.EnsureSuccessStatusCodeAsync();
            var appGroupsObject = JObject.Parse(appGroupsResponse.Content);
            var appGroups = appGroupsObject["value"]?.Value<JArray>();
            return appGroups;
        }

        private async Task<string> GetGraphToken(string accessToken)
        {
            var data = GetGraphTokenPayload(accessToken);
            var authResponse = await _authenticationClient.GetGraphTokenAsync(data);
            await authResponse.EnsureSuccessStatusCodeAsync();
            var jObject = JObject.Parse(authResponse.Content);
            var graphToken = jObject["access_token"]?.Value<string>();
            return graphToken;
        }

        private Dictionary<string, string> GetGraphTokenPayload(string token)
        {
            var data = new Dictionary<string, string>()
            {
                {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                {"client_id", _applicationId},
                {"client_secret", _applicationSecret},
                {"scope", "https://graph.microsoft.com/.default"},
                {"assertion", token},
                {"requested_token_use", "on_behalf_of"}
            };
            return data;
        }
    }
}
