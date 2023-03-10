using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Shared.Clients
{
    public interface IGroupClient
    {
        [Get("/admin/groups")]
        Task<ServiceResponse<IList<Group>>> GetGroups();
    }
}
