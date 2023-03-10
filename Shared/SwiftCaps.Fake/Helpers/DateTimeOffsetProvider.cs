using System;
using SwiftCaps.Helpers.DateTime.Interfaces;

namespace SwiftCaps.Fake.Helpers
{
    public class DateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        private DateTimeOffset _currentDateTime = DateTimeOffset.Now;

        public DateTimeOffset Now => _currentDateTime;

        public void SetCurrentDateTimeOffset(DateTimeOffset currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

    }
}
