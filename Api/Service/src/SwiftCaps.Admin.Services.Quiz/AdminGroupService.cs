using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;

namespace SwiftCaps.Admin.Services
{
    public class AdminGroupService : IAdminGroupService
    {
        private readonly SwiftCapsContext _dbContext;

        public AdminGroupService(SwiftCapsContext swiftCapsContext)
        {
            _dbContext = swiftCapsContext;
        }

        public async Task<Group[]> GetGroupsAsync()
        {
            return await _dbContext.Groups
                                   .OrderBy(g => g.Name)
                                   .Select(g => new Group 
                                   { 
                                       Id = g.Id,
                                       Name = g.Name,
                                       Created = g.Created,
                                       Updated = g.Updated
                                   })
                                   .ToArrayAsync();
        }
    }
}
