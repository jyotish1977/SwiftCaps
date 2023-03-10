using System;
using System.Linq;
using System.Threading.Tasks;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.RestClient.Helpers;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Fake.Services
{
    public class GraphService : IGraphService
    {
        private readonly SwiftCapsContext _swiftCapsContext;

        public GraphService(IAppSettings appSettings, SwiftCapsContext swiftCapsContext)
        {
            _swiftCapsContext = swiftCapsContext;
        }

        public async Task<ServiceResponse<GraphUser>> GetUser(string accessToken, Guid userId)
        {
            var user = _swiftCapsContext.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
                return ServiceResponseHelpers.SuccessServiceResponse<GraphUser>(null, "user not found");

            var graphUser = new GraphUser
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                GroupId = user.GroupId,
                GroupName = user.GroupId.ToString()
            };

            return graphUser.AsServiceResponse();
        }
    }
}
