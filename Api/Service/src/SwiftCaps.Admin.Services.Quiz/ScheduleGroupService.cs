using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using SwiftCaps.Admin.Services.Quiz.Extensions;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Models;
using SCModels = SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;
using Microsoft.Extensions.Configuration;

namespace SwiftCaps.Admin.Services
{
    public class ScheduleGroupService : IScheduleGroupService
    {
        private readonly SwiftCapsContext _context;
        private readonly IConfiguration _configuration;

        public ScheduleGroupService(SwiftCapsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<ScheduleGroupSummary>> GetGroupsAsync(Guid scheduleId)
        {
            Guard.Against.InvalidScheduleGroupListPayload(scheduleId);

            var query = _context.ScheduleGroups
                                 .Include(sg => sg.Group)
                                 .ThenInclude(g => g.Users)
                                 .OrderBy(sg => sg.Group.Name)
                                 .Where(sg => sg.ScheduleId == scheduleId)
                                 .Select(sg => new ScheduleGroupSummary
                                 {
                                     GroupId = sg.Group.Id,
                                     GroupName = sg.Group.Name,
                                     UserCount = sg.Group.Users.Count()
                                 });
            var groups = await query.ToListAsync();
            return groups;
        }

        public async Task<Guid?> CreateGroupAsync(Guid scheduleId, SCModels.Group group)
        {
            Guard.Against.InvalidScheduleGroupListPayload(scheduleId);

            try
            {
                var scheduleExists = await _context.Schedules.AnyAsync(s => s.Id == scheduleId);
                if (!scheduleExists)
                {
                    throw new NotFoundException(scheduleId.ToString(),"Schedule");
                }

                var groupExists = await _context.ScheduleGroups
                                                  .AnyAsync(sg => sg.ScheduleId == scheduleId
                                                                  && sg.GroupId == group.Id);

                if (groupExists)
                {
                    throw new InvalidOperationException("Group already added to schedule.");
                }

                var existingGroup = await _context.Groups.AnyAsync(g => g.Id == group.Id);

                if (!existingGroup)
                {
                    await _context.Groups.AddAsync(group);
                }

                var newScheduleGroup = new ScheduleGroup
                {
                    ScheduleId = scheduleId,
                    GroupId = group.Id
                };
                await _context.ScheduleGroups.AddAsync(newScheduleGroup);
                await _context.SaveChangesAsync();
                return newScheduleGroup.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteGroupAsync(Guid scheduleId, Guid groupId)
        {
            Guard.Against.InvalidScheduleGroupDeletePayload(scheduleId, groupId);
            try
            {
                var scheduleExists = await _context.Schedules.AnyAsync(s => s.Id == scheduleId);
                if(!scheduleExists)
                {
                    throw new NotFoundException(scheduleId.ToString(),"Schedule");
                }

                var scheduleGroup = await _context.ScheduleGroups.SingleOrDefaultAsync(sg => 
                                                sg.ScheduleId == scheduleId
                                                && sg.GroupId == groupId
                                        );
                if(scheduleGroup == null)
                {
                    throw new NotFoundException(groupId.ToString(),"Schedule Group");
                }
                _context.Remove(scheduleGroup);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<SCModels.Group[]> SearchGroupsAsync(Guid scheduleId, string searchString)
        {
            Guard.Against.InvalidScheduleGroupSearchPayload(scheduleId);

            var groupPrefix = _configuration["GroupPrefix"];

            var aadGroups = await GetAadGroups(groupPrefix);

            Func<Microsoft.Graph.Group, bool> searchByDisplayNamePredicate = g =>
            {
                return g.DisplayName.Replace(groupPrefix, string.Empty).ToLower().Contains(searchString?.ToLower() ?? "");
            };
            var filteredGroups = aadGroups.Where(searchByDisplayNamePredicate).ToList();

            var scheduleGroups = await _context.ScheduleGroups
                                             .Where(sg => sg.ScheduleId == scheduleId)
                                             .Select(sg => sg.GroupId.ToString().ToLowerInvariant())
                                             .ToListAsync();

            var groups = filteredGroups.Where(g => !scheduleGroups.Contains(g.Id))
                                      .OrderBy(g => g.DisplayName)
                                      .Select(g => new SCModels.Group
                                      {
                                          Id = Guid.Parse(g.Id),
                                          Name = g.DisplayName
                                      })
                                      .ToArray();

            return groups;
        }

        private async Task<IGraphServiceGroupsCollectionPage> GetAadGroups(string groupPrefix)
        {
            var appSecret = _configuration["ApplicationSecret"];
            var appId = _configuration["ApplicationId"];
            var tenantId = _configuration["TenantId"];

            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                                                    .Create(appId)
                                                    .WithTenantId(tenantId)
                                                    .WithClientSecret(appSecret)
                                                    .Build();

            var authProvider = new ClientCredentialProvider(confidentialClientApplication);
            var graphClient = new GraphServiceClient(authProvider);

            var aadGroups = await graphClient.Groups.Request()
                                                 .Filter($"startswith(displayName,'{groupPrefix}')")
                                                 .Select(g => new
                                                 {
                                                     Id = g.Id,
                                                     Name = g.DisplayName
                                                 })
                                                 .GetAsync();
            return aadGroups;
        }
    }
}
