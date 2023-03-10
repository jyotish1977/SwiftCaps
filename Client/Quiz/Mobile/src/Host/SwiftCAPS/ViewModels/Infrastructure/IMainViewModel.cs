using System;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.ViewModels
{
    public interface IMainViewModel
    {
        Task OnSleep();
        Task OnResume();
        Task OnStart();
        Task OnRun();
        Task<bool> ExecuteLifeCycleAction();
        bool PendingLifeCycleAction { get; }
    }
}
