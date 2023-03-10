using System.Threading.Tasks;
using SwiftCaps.Client.Core.Enums;

namespace SwiftCaps.Client.Core.Interfaces
{
    public interface IAppCacheService<TState> where TState : class
    {
        Task Save();
        Task<bool> Restore();
        Task Save<T>(AppProperty key, T value) where T : class;
        Task<T> Get<T>(AppProperty key) where T : class;
        Task Clear();
        Task Clear(AppProperty key);
        TState State { get; }
    }
}