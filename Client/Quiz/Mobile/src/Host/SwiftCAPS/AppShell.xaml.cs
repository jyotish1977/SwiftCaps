using System.Linq;
using SwiftCaps.ViewModels.Infrastructure;
using Xamarin.Forms;

namespace SwiftCaps
{
    public partial class AppShell: Shell
    {
        public AppShell()
        {
            ViewModelLocator.RegisterRoutes();
            InitializeComponent();
        }

        public void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var routeOut = e?.Current?.Location?.OriginalString?.Split('/')?.LastOrDefault()?.Replace("/", "");

            MessagingCenter.Send<string, ShellNavigatingEventArgs>(nameof(AppShell), routeOut + $"{nameof(OnShellNavigating)}Out", e);

            if (e == null && e.Target == null)
                return;
            
            var routeIn = e.Target.Location.OriginalString.Contains("..") ?
                CurrentState?.Location?.OriginalString?.Split('/')[CurrentState.Location.OriginalString.Split('/').Length - 2] :
                e?.Target?.Location?.OriginalString?.Split('/')?.Last()?.Replace("/", "");
            
            MessagingCenter.Send<string, ShellNavigatingEventArgs>(nameof(AppShell), routeIn + $"{nameof(OnShellNavigating)}In", e);
        }

        public void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
        {

            var route = e?.Current?.Location?.OriginalString?.Split('/')?.LastOrDefault()?.Replace("/", "");
            MessagingCenter.Send<string, ShellNavigatedEventArgs>(nameof(AppShell), route + nameof(OnShellNavigated), e);
        }
    }
}
