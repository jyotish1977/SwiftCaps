using System;
using SwiftCaps.Helpers.DateTime.Interfaces;

namespace SwiftCaps.Helpers.DateTime
{
    public class DateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
