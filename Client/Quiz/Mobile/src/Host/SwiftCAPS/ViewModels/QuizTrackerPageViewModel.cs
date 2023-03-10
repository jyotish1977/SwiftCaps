using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Models.Models;
using Xamarin.Forms;
using Xamariners.Core.Common.Enum;
using Xamariners.Mobile.Core.Infrastructure;

namespace SwiftCaps.ViewModels
{
    public class QuizTrackerPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly ISwiftCapsCacheServices _cacheServices;
        private readonly IAppCacheService<ClientState> _appCacheService;
        public XDelegateTimerCommand GetLeaderBoardCommand { get; }
        public List<LeaderBoard> LeaderBoardList { get; set; }

        public QuizTrackerPageViewModel(ISwiftCapsCacheServices service, IAppCacheService<ClientState> appCacheService)
        {
            _cacheServices = service;
            _appCacheService = appCacheService;

            GetLeaderBoardCommand = new XDelegateTimerCommand(async () => await GetLeaderBoard().ConfigureAwait(true), () => true);
        }

        private async Task GetLeaderBoard()
        {
            try
            {
                var response = await _cacheServices.LeaderBoardCacheService.GetLeaderBoardCache(_appCacheService.State.AppDataPath).ConfigureAwait(false);

                if (!response.IsOK())
                    _errorService.AddError(response);
                else
                {
                    LeaderBoardList = response.Data;
                }

                _errorService.ProcessErrors();
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
            finally
            {
                GetLeaderBoardCommand.ResetTimer();
            }
        }

        private async Task SetData()
        {
            try
            {
                await GetLeaderBoard().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleUIError(ex);
            }
        }
    }
}
