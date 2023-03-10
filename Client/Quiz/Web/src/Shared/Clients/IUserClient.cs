using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Web.Shared.Clients
{
    public interface IUserClient
    {
        [Get("/user/getorcreateuser")]
        Task<ServiceResponse<User>> GetOrCreateUser();
    }
}
