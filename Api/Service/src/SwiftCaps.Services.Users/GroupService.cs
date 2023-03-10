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
    public class GroupService : IGroupService
    {
        private readonly SwiftCapsContext _context;

        public GroupService(SwiftCapsContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Guid>> GetOrCreateGroup(Guid groupId, string groupName)
        {
            var groupResponse = await Get(groupId);

            if (!groupResponse.IsOK())
                return groupResponse.ToServiceResponse<Group, Guid>();

            var group = groupResponse.GetData();

            if (group != null) return group.Id.AsServiceResponse();

            var createGroupResponse = await Create(new Group
            {
                Id = groupId,
                Name = groupName
            });

            return !createGroupResponse.IsOK() ? createGroupResponse.ToServiceResponse<Group, Guid>() : createGroupResponse.Data.Id.AsServiceResponse();
        }

        
        private async Task<ServiceResponse<Group>> Create(Group group)
        {
            try
            {
                await _context.AddAsync(group);

                var created = await _context.SaveChangesAsync();

                return group.AsServiceResponse();
            }
            catch(Exception ex)
            {
                return ServiceResponseHelpers.UnhandledServiceResponse<Group>(ex);
            }
        }

        private async Task<ServiceResponse<Group>> Get(Guid groupId)
        {
            try
            {
                var group = await _context.Groups.FindAsync(groupId);
                var response = group.AsServiceResponse();

                if (group == null)
                    response.Message = "Group not found";

                return response;
            }
            catch (Exception ex)
            {
                return ServiceResponseHelpers.UnhandledServiceResponse<Group>(ex);
            }
        }
    }
}
