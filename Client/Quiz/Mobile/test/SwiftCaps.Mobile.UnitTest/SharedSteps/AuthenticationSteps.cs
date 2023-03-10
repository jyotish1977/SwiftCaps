using System;
using Shouldly;
using SwiftCAPS.Mobile.UnitTest.Infrastructure;
using SwiftCaps.ViewModels;
using SwiftCaps.Views;
using TechTalk.SpecFlow;
using Unity;
using Xamariners.RestClient.Helpers.Models;
using Xamariners.UnitTest.Xamarin.SharedSteps;

namespace SwiftCAPS.Mobile.UnitTest.SharedSteps
{
    [Binding]
    public class AuthenticationSteps : StepBase
    {
        public AuthenticationSteps(ScenarioContext scenarioContext)
            : base(scenarioContext) { }


        [Given(@"I am an unauthenticated user")]
        [Then(@"I am unauthenticated")]
        public void GivenIAmAnUnauthenticatedUser()
        {
            Services.AppCache.State.IsAuthenticated.ShouldBeFalse();
        }


        [Given(@"I am authenticated")]
        [Then(@"I am authenticated")]
        public void ThenIAmAuthenticated()
        {
            Services.AppCache.State.IsAuthenticated.ShouldBeTrue();
        }

        [When(@"I login with Azure AD")]
        public async void WhenILoginWithAzureAD()
        {
            var loginViewModel = App.Container.Resolve<LoginPageViewModel>();

            loginViewModel.LoginCommand.Execute();

            if (loginViewModel.LoginCommand is { IsValid: true })
                loginViewModel.LoginCommand?.WaitHandle.WaitOne();

            var authToken = new AuthToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IssuedAt = DateTime.UtcNow,
                AccessToken = "test",
                ExpiresIn = 30 * 24 * 60 * 60
            };

            if (Services.AppCache.State.IsAuthenticated)
            {
                Services.AppCache.State.Credential.AuthToken = authToken;
                await Services.AppCache.Save().ConfigureAwait(false);
            }
        }

        [When(@"I am an authenticated user with mobilenumber ""(.*)"" and password ""(.*)""")]
        [Given(@"I am an authenticated user with mobilenumber ""(.*)"" and password ""(.*)""")]
        [When(@"I am an authenticated user with email ""(.*)"" and password ""(.*)""")]
        [Given(@"I am an authenticated user with email ""(.*)"" and password ""(.*)""")]
        public void GivenIAmAnAuthenticatedUserWithMobilenumberAndPassword(string mobilenumber, string password)
        {
            WhenILoginWithAzureAD();
            ThenIAmAuthenticated();
        }

        [Given(@"I am authenticated on the hamburger item ""(.*)"" with mobilenumber ""(.*)"" and password ""(.*)""")]
        public void GivenIAmAuthenticatedOnTheHamburgerItemViewWithMobilenumberAndPassword(string pageName, string mobilenumber, string password)
        {
            var navSteps = new NavigationSteps(_scenarioContext);

            WhenILoginWithAzureAD();
            ThenIAmAuthenticated();

            navSteps.GivenIAmOnTheView(nameof(LoginPage));
            navSteps.GivenIAmOnTheView(pageName);
        }
    }
}
