using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SwiftCaps.Client.Shared.Services.Abstraction;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Models;

namespace SwiftCAPS.Admin.Web.Shared.States
{
    public class UserState
    {
        public IReadOnlyList<Application> Applications { get; private set; }
        public event Action OnChange;

        private readonly IUserApplicationService _userApplicationService;

        public UserState(IUserApplicationService userApplicationService)
        {
            _userApplicationService = userApplicationService;
            Applications = new List<Application>();
        }

        public async Task LoadApplications()
        {
            if (Applications.Any())
            {
                return;
            }
            Applications = await _userApplicationService.GetApplicationsAsync();
            NotifyStateChanged();
        }

        private string activeQuizDetailTab = DetailsPivotType.General.ToString();
        public string ActiveQuizDetailTab
        {
            get => activeQuizDetailTab;
            set
            {
                activeQuizDetailTab = value;
                NotifyStateChanged();
            }
        }

        private int activeQuizDetailSection = 1;
        public int ActiveQuizDetailSection
        {
            get => activeQuizDetailSection;
            set
            {
                activeQuizDetailSection = value;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
