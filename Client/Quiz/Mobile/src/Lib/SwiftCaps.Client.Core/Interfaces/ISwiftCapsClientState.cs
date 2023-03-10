using Xamariners.Core.Interface;

namespace SwiftCaps.Client.Core.Interfaces
{
    public interface ISwiftCapsClientState : IClientState
    {
         string AppDataPath { get; set; }
    }
}
