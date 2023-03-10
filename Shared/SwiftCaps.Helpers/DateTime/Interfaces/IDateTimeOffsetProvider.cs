using System;
namespace SwiftCaps.Helpers.DateTime.Interfaces
{
    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset Now { get; }
    }
}
