using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SwiftCaps.Client.Shared.Services.Abstraction;
using SwiftCaps.Models.Models;

namespace SwiftCAPS.Web.Shared.State
{
    public class UserState
    {
        public User User { get; private set; }
        public IReadOnlyList<Application> Applications { get; private set; }

        private readonly IUserApplicationService _userApplicationService;

        public UserState(IUserApplicationService userApplicationService)
        {
            _userApplicationService = userApplicationService;
            Applications = new List<Application>();
        }

        public UserQuiz UserQuiz { get; private set; }

        public event Action OnChange;

        public void SetUser(User user)
        {
            User = user;
            NotifyStateChanged();
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

        public void SetUserQuiz(UserQuiz userQuiz)
        {
            UserQuiz = userQuiz;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

    }
}
