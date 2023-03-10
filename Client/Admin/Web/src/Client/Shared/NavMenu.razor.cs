using System.Collections.Generic;
using BlazorFluentUI;
using BlazorFluentUI.Routing;

namespace SwiftCAPS.Admin.Web.Client.Shared
{
    public partial class NavMenu
    {
        protected List<NavBarItem> _items;

        protected override void OnInitialized()
        {
            _items = new List<NavBarItem>
            {
                new NavBarItem
                {
                    ItemType=ContextualMenuItemType.Normal,
                    Style="background-color: blue !important",
                    Text= "Quiz",
                    Url="",
                    NavMatchType= NavMatchType.RelativeLinkOnly,
                    Id="",
                    IconName = "Inbox",
                    Key="1"
                },
                new NavBarItem
                {
                    Text= "Schedule quiz",
                    Url="schedules",
                    NavMatchType= NavMatchType.RelativeLinkOnly,
                    Id="schedules",
                    IconName = "Calendar",
                    Key="2"
                },
                new NavBarItem()
                {
                    Text= "Reporting",
                    Id="Reporting",
                    IconName="ReportDocument",
                    Key="3",
                    IsExpanded=true,
                    Items = new List<NavBarItem>
                            {
                                new NavBarItem
                                {
                                    Text="Leaderboard",
                                    Url="reporting/leaderboard",
                                    NavMatchType=NavMatchType.RelativeLinkOnly,
                                    Id="reporting/leaderboard",
                                    IconName="ClipboardList",
                                    Key="4"
                                },
                                new NavBarItem
                                {
                                    Text="Group % Avg",
                                    Url="reporting/groupprogress",
                                    NavMatchType=NavMatchType.RelativeLinkOnly,
                                    Id="reporting/groupprogress",
                                    IconName="Group",
                                    Key="5"
                                },
                                new NavBarItem
                                {
                                    Text="Quiz % Avg",
                                    Url="reporting/quizaverage",
                                    NavMatchType=NavMatchType.RelativeLinkOnly,
                                    Id="reporting/quizaverage",
                                    IconName="SurveyQuestions",
                                    Key="6"
                                },
                                new NavBarItem
                                {
                                    Text="Group %",
                                    Url="reporting/groupaverage",
                                    NavMatchType=NavMatchType.RelativeLinkOnly,
                                    Id="reporting/groupaverage",
                                    IconName="Group",
                                    Key="7"
                                }
                            }
                }
            };

            base.OnInitialized();

        }
    }
}
