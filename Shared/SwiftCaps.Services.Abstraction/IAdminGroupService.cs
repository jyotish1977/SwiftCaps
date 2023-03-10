using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IAdminGroupService
    {
        public Task<Group[]> GetGroupsAsync();
    }
}
