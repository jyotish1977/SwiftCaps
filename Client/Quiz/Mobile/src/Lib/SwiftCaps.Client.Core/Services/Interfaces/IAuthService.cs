using System.Threading.Tasks;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Client.Core.Services.Interfaces
{
    public interface IAuthService
    {
        #region Buyer API

        Task<ServiceResponse<bool>> Login();

        Task<ServiceResponse<bool>> Logout();

        #endregion
        
    }
}
