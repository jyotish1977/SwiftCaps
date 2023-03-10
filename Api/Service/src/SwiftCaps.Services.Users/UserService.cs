using System;
using System.Threading.Tasks;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.RestClient.Helpers;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.User
{
    public class UserService : IUserService
    {
        private readonly SwiftCapsContext _dbContext;
        private readonly IGroupService _groupService;
        private readonly IGraphService _graphService;

        public UserService(SwiftCapsContext dbContext, IGroupService groupService, 
            IGraphService graphService)
        {
            _dbContext = dbContext;
            _groupService = groupService;
            _graphService = graphService;
        }

        public async Task<ServiceResponse<Models.Models.User>> GetOrCreateUser(Guid userId, string accessToken = null)
        {
            // TODO: move this in the user service
            var userResponse = await GetUser(userId);

            if (!userResponse.IsOK())
                throw new Exception(userResponse.ErrorMessage);

            var user = userResponse.Data;

            if (user != null) return userResponse;

            var graphUserResponse = await _graphService.GetUser(accessToken, userId);

            if (!graphUserResponse.IsOK())
                return graphUserResponse.ToServiceResponse<GraphUser, Models.Models.User>();

            var graphUser = graphUserResponse.GetData();

            var createGroupResponse = await _groupService.GetOrCreateGroup(graphUser.GroupId, graphUser.GroupName);

            if (!createGroupResponse.IsOK())
                return createGroupResponse.ToServiceResponse<Guid, Models.Models.User>();

            var newUser = new Models.Models.User
            {
                Id = graphUser.UserId,
                FirstName = graphUser.FirstName,
                LastName = graphUser.LastName,
                FullName = graphUser.FullName,
                Email = graphUser.Email,
                GroupId = createGroupResponse.GetData()
            };

            var newUserResponse = await CreateUser(newUser);

            return newUserResponse;
        }

        private async Task<ServiceResponse<Models.Models.User>> GetUser(Guid userId)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                var response = user.AsServiceResponse();

                if (user == null)
                    response.Message = "User not found";

                return response;
            }
            catch (Exception ex)
            {
                return ServiceResponseHelpers.UnhandledServiceResponse<Models.Models.User>(ex);
            }
        }

        private async Task<ServiceResponse<Models.Models.User>> CreateUser(Models.Models.User newUser)
        {
            try
            {
                await _dbContext.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                return newUser.AsServiceResponse();
            }
            catch (Exception ex)
            {
                return ServiceResponseHelpers.UnhandledServiceResponse<Models.Models.User>(ex);
            }
        }
    }
}
