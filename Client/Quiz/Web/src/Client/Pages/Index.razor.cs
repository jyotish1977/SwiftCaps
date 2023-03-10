using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Client.Shared.Models;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCAPS.Web.Shared.Clients;
using SwiftCAPS.Web.Shared.State;
using Xamariners.Core.Interface;

namespace SwiftCAPS.Web.Client.Pages
{
    [Authorize(Roles = nameof(RoleConstants.User))]
    public partial class Index
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] public IUserClient UserClient { get; set; }
        [Inject] public IQuizClient QuizClient { get; set; }
        [Inject] public UserState UserState { get; set; }
        /*[Inject]*/ public ILogger Logger { get; set; }

        public UserQuiz Quiz { get; set; } = new UserQuiz();
        public List<UserQuiz> UserQuizzes { get; set; }
        public string ErrorMessage { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)  return;
            try
            {
                await LoadUserQuizzes();
            }
            catch (Exception e)
            {
                ErrorMessage = "There was an error retrieving quizzes, please try again.";
                Logger?.LogAction(e);
            }
            finally
            {
                StateHasChanged();
            }
        }

        private async Task LoadUserQuizzes()
        {
            var userResponse = await UserClient.GetOrCreateUser();
            if (!userResponse.IsOK())
            {
                throw new Exception(string.Join(',', userResponse.Errors));
            }

            UserState.SetUser(userResponse.Data);

            var quizzes = await QuizClient.GetAvailableUserQuizzes(new UserQuizRequest
            {
                ClientLocalDateTime = DateTimeOffset.Now,
                UserId = userResponse.Data.Id
            });

            if (!quizzes.IsOK())
            {
                throw new Exception(string.Join(',', quizzes.Errors));
            }

            UserQuizzes = quizzes.Data.ToList();
        }

        private async Task QuizClickHandler(UserQuiz quiz)
        {
            var response = await QuizClient.AddUserQuiz(quiz);

            if (response.IsOK())
            {
                UserState.SetUserQuiz(response.Data);
                NavigationManager.NavigateTo("/quiz");
            }
            else
                throw new Exception(response.ErrorMessage);
        }
    }
}
