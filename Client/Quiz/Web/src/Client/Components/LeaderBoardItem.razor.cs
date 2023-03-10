using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Models;

namespace SwiftCAPS.Web.Client.Components
{
    public partial class LeaderBoardItem
    {
        [Parameter]
        public LeaderBoard LeaderBoard { get; set; }
    }
}
