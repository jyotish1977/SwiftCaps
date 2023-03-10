using System;
using System.Windows.Input;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Client.Core.Services.Interfaces;
using SwiftCaps.Values;
using SwiftCaps.Views;
using Unity;
using Xamariners.Mobile.Core.Interfaces;

namespace SwiftCaps.Infrastructure
{
    public class BaseContentPage : ContentPage
    {
        public static readonly BindableProperty ShowDropdownProperty = BindableProperty.Create(
            nameof(ShowDropdown),
            typeof(bool),
            typeof(BaseContentPage),
            false);

        public static readonly BindableProperty ShowDropdownCommandProperty = BindableProperty.Create(
            nameof(ShowDropdownCommand),
            typeof(ICommand),
            typeof(BaseContentPage));

        public static readonly BindableProperty DropDownListProperty = BindableProperty.Create(
            nameof(DropDownList),
            typeof(List<DisplayPage>),
            typeof(BaseContentPage),
            null);

        public static readonly BindableProperty ToolbarItemCommandProperty = BindableProperty.Create(
            nameof(ToolbarItemCommand),
            typeof(ICommand),
            typeof(BaseContentPage));

        public bool ShowDropdown
        {
            get => (bool)GetValue(ShowDropdownProperty);
            set => SetValue(ShowDropdownProperty, value);
        }

        public ICommand ShowDropdownCommand
        {
            get => (ICommand)GetValue(ShowDropdownCommandProperty);
            set => SetValue(ShowDropdownCommandProperty, value);
        }

        public List<DisplayPage> DropDownList
        {
            get => (List<DisplayPage>)GetValue(DropDownListProperty);
            set => SetValue(DropDownListProperty, value);
        }

        public ICommand ToolbarItemCommand
        {
            get => (ICommand)GetValue(ToolbarItemCommandProperty);
            set => SetValue(ToolbarItemCommandProperty, value);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetNavigationDropDown();
        }

        private void SetNavigationDropDown()
        {
            //TODO: move all this to VM

            if (ShowDropdownCommand is null)
                ShowDropdownCommand = new Command(DisplayDropdown);

            if (ToolbarItemCommand is null)
                ToolbarItemCommand = new Command<string>(async (navParam) => await ToolbarItemTapped(navParam));

            if (DropDownList is null)
                DropDownList = new List<DisplayPage>()
                {
                    new DisplayPage()
                    {
                        Title = "QUIZ OVERVIEW",
                        NavParam = nameof(QuizListPage)
                    },
                    new DisplayPage()
                    {
                        Title = "QUIZ TRACKER",
                        NavParam = nameof(QuizTrackerPage)
                    },
                    new DisplayPage()
                    {
                        Title = "LOGOUT",
                        NavParam = null
                    }
                };
        }

        private void DisplayDropdown()
        {
            ShowDropdown = !ShowDropdown;
        }

        protected virtual async Task ToolbarItemTapped(string navParam)
        {
            ShowDropdown = false;

            // TODO: treat as hamburger, ie navigate on same top level
            // TODO: change from loginpage as root to quiz list when authenticated
            var currentPage = BootStrapper.Container.Resolve<INavigationService>().CurrentPage.GetType().Name;

            if (!string.Equals(currentPage, navParam, StringComparison.CurrentCultureIgnoreCase))
            {
                // root 

                switch (navParam)
                {
                    // LOGOUT 
                    case null:
                    {
                        var isLogoutOk = true;
                        if (Shell.Current.CurrentPage.GetType() == typeof(QuizPage))
                        {
                            // Handle Logout from QuizPage
                            isLogoutOk = await HandleQuizPageNavigation().ConfigureAwait(false);
                        }

                        if (isLogoutOk)
                        {
                            await Shell.Current.GoToAsync($"..").ConfigureAwait(false);

                            // Logout - reset root to login page

                            await BootStrapper.Container.Resolve<ISwiftCapsCacheServices>()
                                .DeleteDatabase(BootStrapper.Container.Resolve<IAppCacheService<ClientState>>()
                                    .State
                                    .AppDataPath)
                                .ConfigureAwait(false);

                            await BootStrapper.Container.Resolve<IAuthService>().Logout().ConfigureAwait(true);
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.GoToAsync(Constants.ShellNavigation.LoginPagePath)
                                    .ConfigureAwait(false);
                            });
                        }
                    }
                        break;

                    // QUIZ OVERVIEW 
                    case nameof(QuizListPage):
                    {
                        if (Shell.Current.CurrentPage.GetType() != typeof(QuizListPage))
                        {
                            // Am I inside QuizPage or not?
                            if (Shell.Current.CurrentPage.GetType() == typeof(QuizPage))
                            {
                                // Handle Navigation from QuizPage
                                var result = await HandleQuizPageNavigation();
                                if (result)
                                    await Shell.Current.GoToAsync($"..");
                            }
                            else
                            {
                                // Go back to QuizListPage
                                await Shell.Current.GoToAsync($"..");
                            }
                        }
                    }
                        break;

                    // QUIZ TRACKER 
                    case nameof(QuizTrackerPage):
                    {
                        if (Shell.Current.CurrentPage.GetType() != typeof(QuizTrackerPage))
                        {
                            // Am I inside QuizPage or not?
                            if (Shell.Current.CurrentPage.GetType() == typeof(QuizPage))
                            {
                                // Handle Navigation from QuizPage
                                var result = await HandleQuizPageNavigation();
                                if (result)
                                    await Shell.Current.GoToAsync(
                                        $"../{Constants.ShellNavigation.QuizTrackerPagePath}");
                            }
                            else
                            {
                                // Go to QuizTrackerPage
                                await Shell.Current.GoToAsync($"{Constants.ShellNavigation.QuizTrackerPagePath}");
                            }
                        }
                    }
                        return;
                }
            }
        }

        private async Task<bool> HandleQuizPageNavigation()
        {
            if (Shell.Current.CurrentPage.GetType() == typeof(QuizPage))
            {
                var result = await BootStrapper.Container.Resolve<IPopupInputService>()
                    .ShowMessageOkCancelAlertPopup(
                        $"Quiz in progress!",
                        $"Your progress will be saved.", "OK", "CANCEL");
                if (!result)
                    return false;
            }

            return true;
        }

        protected override bool OnBackButtonPressed()
        {
            // Handle QuizPage Back Navigation (Android only)
            if (Device.RuntimePlatform == Device.Android)
            {
                // For Android Back Button
                if (Shell.Current.CurrentPage.GetType() == typeof(QuizPage))
                {
                    // Confirm the Quiz Page backward navigation
                    HandleQuizPageNavigation()
                        .ContinueWith((result) =>
                        {
                            if (result.Result)
                            {
                                // User Confirmed backward navigation
                                Device.BeginInvokeOnMainThread(() => { Shell.Current.GoToAsync(".."); });
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                    // Cancel the default backward navigation
                    return true;
                }
            }

            // Execute as usual for other platforms
            return base.OnBackButtonPressed();
        }
    }

    public class DisplayPage
    {
        public string Title { get; set; }
        public string NavParam { get; set; }
    }
}