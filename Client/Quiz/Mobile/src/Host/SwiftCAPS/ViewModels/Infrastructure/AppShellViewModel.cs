using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Views;
using Xamarin.Forms;
using Xamariners.Mobile.Core.Helpers;
using Xamariners.Mobile.Core.Infrastructure;

namespace SwiftCaps.ViewModels
{
    public class AppShellViewModel : ViewModelBase
    {
        private readonly ISwiftCapsApiServices _services;

        public string Username { get; set; }
        public string Fullname { get; set; }

        public XDelegateCommand GoToLoginCommand { get; }

        public AppShellViewModel(ISwiftCapsApiServices services)
        {
            _services = services;

            GoToLoginCommand = new XDelegateCommand(async () => await GoToLogin(), () => true);

            new InitializedViewModelBase(UpdateHeader).Invoke();
        }

        private void UpdateHeader()
        {
            if (_services.AppCache.State.IsAuthenticated)
            {
                Username = _services.AppCache.State.Member.Username;
                Fullname = $"{_services.AppCache.State.Member.FirstName} {_services.AppCache.State.Member.LastName}";
            }
        }

        public async Task GoToLogin()
        {
            await _navigationService.GoToAsync(nameof(LoginPage).ToLower());
        }
    }
}
