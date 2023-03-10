using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refit;

namespace SwiftCaps.Services.User.Clients
{
    public interface IAuthenticationClient
    {
        [Post("/token")]
        Task<ApiResponse<string>> GetGraphTokenAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string,string> data);
    }
}
