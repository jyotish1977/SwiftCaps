using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Models;

namespace SwiftCAPS.Blazor.Components
{
    public partial class SidePanel
    {
        [Parameter]
        public IReadOnlyList<Application> Applications { get; set; }

        [Parameter]
        public bool IsAppPanelOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsAppPanelOpenChanged { get; set; }

        private Task PanelDismiss()
        {
            IsAppPanelOpen = false;
            return IsAppPanelOpenChanged.InvokeAsync(IsAppPanelOpen);
        }
    }
}
