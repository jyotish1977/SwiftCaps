using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorFluentUI;
using BlazorFluentUI.Routing;

namespace SwiftCAPS.Web.Client.Shared
{
    public partial class NavMenu
    {
        private List<NavBarItem> _items;

        protected override Task OnInitializedAsync()
        {

            _items = new List<NavBarItem> 
            {
                new NavBarItem
                {
                    Text= "Quiz",
                    Url="/",
                    NavMatchType= NavMatchType.AnchorOnly,
                    Id="quiz",
                    IconName = "Inbox",
                    Key="1"
                },
                new NavBarItem
                {
                    Text= "Leaderboard",
                    Url="/leaderboard",
                    NavMatchType= NavMatchType.AnchorOnly,
                    Id="leaderboard",
                    IconName = "Send",
                    Key="2"
                }
            };

            return base.OnInitializedAsync();
        }

    }
}
