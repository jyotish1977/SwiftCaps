using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SwiftCAPS.Web.Client.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private SignOutSessionStateManager SignOutManager { get; set; }

        private string GetEmail(AuthenticationState authState)
        {
            return authState.User.Claims.First(c => c.Type == "preferred_username").Value;
        }

        private async Task BeginLogout(MouseEventArgs args)
        {
            await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("authentication/logout");
        }
    }
}
