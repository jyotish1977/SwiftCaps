using BlazorFluentUI;
using Microsoft.AspNetCore.Components;
using SwiftCAPS.Admin.Web.Shared.States;

namespace SwiftCAPS.Admin.Web.Client.Shared
{
    public partial class MainLayout
    {
        [Inject] private UserState State { get; set; }

        protected bool _isMenuPanelOpen;
        protected bool _isAppPanelOpen;

        [CascadingParameter] public ResponsiveMode CurrentMode { get; set; }

        public int BodyPadding => (int)CurrentMode <= (int)ResponsiveMode.Medium ? 20 : 0;

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
