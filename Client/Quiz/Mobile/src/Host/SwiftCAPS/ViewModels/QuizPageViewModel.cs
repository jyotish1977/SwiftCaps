using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Enums;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Extensions;
using SwiftCaps.Models.Models;
using Xamarin.Forms;
using Xamariners.Core.Common.Enum;
using Xamariners.Mobile.Core.Infrastructure;

namespace SwiftCaps.ViewModels
{
    public class QuizPageViewModel : ViewModelBase
    {
        private readonly ISwiftCapsCacheServices _cacheServices;

        private readonly IAppCacheService<ClientState> _appCacheService;

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    (SubmitSectionCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public UserQuiz UserQuiz { get; set; }
        public bool IsQuizCompleted { get; set; }

        public XDelegateCommand SubmitSectionCommand { get; set; }
        public XDelegateCommand ValidateSectionCommand { get; set; }
        public XDelegateCommand OpenInfoCommand { get; set; }
        public XDelegateCommand CloseInfoCommand { get; set; }

        // TODO : move the indicator to its own control or us xamarin forms carousel and built in indicator
        public bool IsLessSymbolVisible => SelectedQuestionIndex > 0;
        public bool IsGreaterSymbolVisible => SelectedQuestionIndex < CurrentSection?.Questions?.Count - 1;

        private int _currentQuizSectionIndex;

        public int CurrentQuizSectionIndex
        {
            get => _currentQuizSectionIndex;
            set => SetProperty(ref _currentQuizSectionIndex, value, () =>
            {
                RaisePropertyChanged(nameof(CurrentSection));
                RaisePropertyChanged(nameof(SectionCountText));
                RaisePropertyChanged(nameof(IsLessSymbolVisible));
                RaisePropertyChanged(nameof(IsGreaterSymbolVisible));

                // New section - Question index is 0
                SelectedQuestionIndex = 0;
            });
        }

        private int _selectedQuestionIndex = -1;

        public int SelectedQuestionIndex
        {
            get => _selectedQuestionIndex;
            set => SetProperty(ref _selectedQuestionIndex, value, () =>
            {
                RaisePropertyChanged(nameof(CurrentQuestion));
                RaisePropertyChanged(nameof(QuestionCountText));
                RaisePropertyChanged(nameof(SectionCountText));
                RaisePropertyChanged(nameof(IsLessSymbolVisible));
                RaisePropertyChanged(nameof(IsGreaterSymbolVisible));
            });
        }

        public QuizSection CurrentSection => UserQuiz?.Schedule?.Quiz?.QuizSections?[CurrentQuizSectionIndex];
        public string InfoMarkdown => UserQuiz?.Schedule?.Quiz?.InfoMarkdown;

        public Question CurrentQuestion => CurrentSection?.Questions?[SelectedQuestionIndex];

        public string SectionCountText =>
            $"Section {CurrentQuizSectionIndex + 1} of {UserQuiz?.Schedule?.Quiz?.QuizSections?.Count()}";

        public string QuestionCountText => $"{SelectedQuestionIndex + 1}/{CurrentSection?.Questions?.Count()}";

        public QuizAnswersLayout QuizLayoutStyle { get; }

        public QuizPageViewModel(ISwiftCapsCacheServices cacheServices, IAppSettings appSettings,
            IAppCacheService<ClientState> appCacheService)
        {
            _cacheServices = cacheServices;
            _appCacheService = appCacheService;

            // DEBUG: to skip between sections, set the SubmitSectionCommand CanExecute () => true
            SubmitSectionCommand = new XDelegateCommand(async () => await SubmitSection(),
                () => CurrentSection is { IsValid: true } && !IsBusy, () => true);
            ValidateSectionCommand = new XDelegateCommand(ValidateSection, () => true);
            OpenInfoCommand = new XDelegateCommand(async () => await OpenInfo(), () => true);
            CloseInfoCommand = new XDelegateCommand(CloseInfo, () => true);

            QuizLayoutStyle = appSettings.QuizAnswersLayout;
        }

        private void ValidateSection()
        {
            try
            {
                CurrentSection.IsValid = !CurrentSection.Questions.Any(q => q.QuizAnswers.Any(a => !a.IsValid));

                IsQuizCompleted = UserQuiz.Schedule.Quiz.QuizSections.All(s => s.IsValid);

                SubmitSectionCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
            finally
            {
                ValidateSectionCommand.Reset();
            }
        }

        private async Task OpenInfo()
        {
            if (InfoMarkdown != null)
            {
                try
                {
                    await _popupInputService.ShowInfoMarkdownViewPopup(InfoMarkdown,
                        ImageSource.FromResource("SwiftCaps.Resources.Images.close.png",
                            typeof(ImageResourceExtension).GetTypeInfo().Assembly)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    HandleUIError(ex);
                }
                finally
                {
                    OpenInfoCommand.Reset();
                }
            }
        }

        private void CloseInfo()
        {
            try
            {
                _popupInputService.CloseLastPopup();
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
            finally
            {
                CloseInfoCommand.Reset();
            }
        }

        private async Task SubmitSection()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            try
            {
                if (!IsQuizCompleted)
                {
                    // move next
                    SetupQuizSection();
                }
                else
                {
                    await SubmitQuiz().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
            finally
            {
                ValidateSection();
                IsBusy = false;
                SubmitSectionCommand.Reset();
            }
        }

        private async Task SubmitQuiz()
        {
            // save quiz with API
            var quizName = UserQuiz.Schedule?.Quiz?.Name ?? "";

            var schedule = UserQuiz.Schedule;
            UserQuiz.Completed = DateTime.UtcNow;
            UserQuiz.Schedule = null;

            var response = await _cacheServices.QuizCacheService
                .SaveUserQuiz(_appCacheService.State.AppDataPath, UserQuiz).ConfigureAwait(false);

            UserQuiz.Schedule = schedule;
            if (response.HttpStatus == HttpStatusCode.Created || response.HttpStatus == HttpStatusCode.OK)
            {
                _currentQuizSectionIndex = 0;
                _selectedQuestionIndex = -1;
                await _cacheServices.LeaderBoardCacheService.SetLeaderBoardCache(_appCacheService.State
                    .AppDataPath, _appCacheService.State.Member.Id).ConfigureAwait(false);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    // show quiz completed popup
                    await _popupInputService.ShowMessageOkAlertPopup(
                        $"Congratulations {_appCacheService?.State?.Member?.FirstName}",
                        $"You successfully completed the {quizName} Quiz", "OK").ConfigureAwait(true);

                    // go back to Quiz List page
                    await _navigationService.GoToRootAsync().ConfigureAwait(true);
                });
            }
            else
            {
                _errorService.AddError(response);
                _errorService.ProcessErrors();
            }
        }

        protected override async Task OnShellNavigatingIn(string sender, ShellNavigatingEventArgs args)
        {
            await base.OnShellNavigatingIn(sender, args).ConfigureAwait(true);

            try
            {
                if (_navigationService.NavigationDirection == NavigationDirection.Forward)
                {
                    CurrentQuizSectionIndex = 0;
                    SelectedQuestionIndex = 0;
                    SubmitSectionCommand.RaiseCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
        }

        private void SetupQuizSection()
        {
            // check if reached end of quiz
            if (CurrentQuizSectionIndex < UserQuiz.Schedule.Quiz.QuizSections.Count() - 1)
            {
                // Set Current Section
                CurrentQuizSectionIndex++;
            }

            SubmitSectionCommand.RaiseCanExecuteChanged();
        }
    }
}