using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Client.Shared.Models;
using SwiftCAPS.Web.Shared.State;

namespace SwiftCAPS.Web.Client.Shared
{
    public partial class MainLayout
    {
        [Inject] private UserState State { get; set; }

        private string _allowedRoles = nameof(RoleConstants.User);

        protected bool _isMenuPanelOpen;
        protected bool _isAppPanelOpen;
        [CascadingParameter] public ResponsiveMode CurrentMode { get; set; }

        public int BodyPadding => (int)CurrentMode <= (int)ResponsiveMode.Medium ? 10 : 0;

        private void ShowMenu()
        {
            _isMenuPanelOpen = true;
        }

        private void HideMenu()
        {
            _isMenuPanelOpen = false;
        }

        private void AppMenuClick()
        {
            _ = State.LoadApplications();
            _isAppPanelOpen = true;
        }


    }
}
