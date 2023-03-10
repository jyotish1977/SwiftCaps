using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Models.Models;
using Xamarin.Forms;
using Xamariners.Mobile.Core.Infrastructure;
using static SwiftCaps.Values.Constants;

namespace SwiftCaps.ViewModels
{
    public class QuizListPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly ISwiftCapsCacheServices _cacheServices;
        private readonly IAppCacheService<ClientState> _appCacheService;

        public List<UserQuiz> UserQuizzes { get; set; }
        public XDelegateCommand<UserQuiz> GoToQuizCommand { get; }
        public XDelegateTimerCommand GetAvailableUserQuizzesCommand { get; }
       
        public QuizListPageViewModel(ISwiftCapsCacheServices cacheServices, IAppCacheService<ClientState> appCacheService)
        {
            _cacheServices = cacheServices;
            _appCacheService = appCacheService;

            GetAvailableUserQuizzesCommand = new XDelegateTimerCommand(async () => await GetAvailableUserQuizzes().ConfigureAwait(false), () => _appCacheService.State.IsAuthenticated);

            GoToQuizCommand = new XDelegateCommand<UserQuiz>(async quiz => await GoToQuiz(quiz).ConfigureAwait(false), quiz => true, quiz => true);
        }

        protected override async Task OnShellNavigatingIn(string sender, ShellNavigatingEventArgs args)
        {
            await base.OnShellNavigatingIn(sender, args).ConfigureAwait(true);

            await SetData().ConfigureAwait(false);
        }

        protected override async Task OnShellNavigatingOut(string sender, ShellNavigatingEventArgs args)
        {
            if (args.Target.Location.OriginalString.Equals(ShellNavigation.LoginPagePath) && _appCacheService.State.IsAuthenticated) //This is required for Android Back button
            {
                args.Cancel();
                return;
            }

            await base.OnShellNavigatingOut(sender, args).ConfigureAwait(true);
        }

        private async Task GoToQuiz(UserQuiz selectedUserQuiz)
        {
            try
            {
                if (selectedUserQuiz.Completed.HasValue)
                    return;

                var response = await _cacheServices.QuizCacheService.GetUserQuizCache(_appCacheService.State.AppDataPath, selectedUserQuiz.Id)
                    .ConfigureAwait(false);

                if (!response.IsOK())
                    _errorService.AddError(response);
                else
                    await _navigationService.GoToAsync(ShellNavigation.QuizPagePath,
                        (QuizPageViewModel vm) => vm.UserQuiz = response.Data).ConfigureAwait(false);

                _errorService.ProcessErrors();
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
            finally
            {
                GoToQuizCommand.Reset();
            }
        }
        
        private async Task GetAvailableUserQuizzes()
        {
            try
            {
                var response = await _cacheServices.QuizCacheService.GetUserQuizzesCache(_appCacheService.State.AppDataPath).ConfigureAwait(false);

                if (!response.IsOK())
                    _errorService.AddError(response);
                else
                    UserQuizzes = response.Data;

                _errorService.ProcessErrors();
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
            finally
            {
                GetAvailableUserQuizzesCommand.ResetTimer();
            }
        }

        private async Task SetData()
        {
             await GetAvailableUserQuizzes().ConfigureAwait(false);
        }
    }
}
