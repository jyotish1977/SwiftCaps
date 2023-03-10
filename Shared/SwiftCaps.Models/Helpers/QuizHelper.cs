using System;
using System.Text.RegularExpressions;
using SwiftCaps.Models.Constants;
using Xamariners.Core.Common.Helpers;

namespace SwiftCaps.Models.Helpers
{
    public static class QuizHelper
    {

        public static DateTimeOffset GetMonthlyExpiry(DateTimeOffset currentDate, bool isShift)
        {

            var returnDate = isShift
                ? new DateTimeOffset(GetDateWithNewTime(currentDate.DateTime, TimeshiftConstants.TIME_SHIFT_END_TIME), TimeZoneInfo.Local.GetUtcOffset(currentDate)).AddMonths(1).LastDayOfMonth(true)
                : new DateTimeOffset(GetDateWithNewTime(currentDate.DateTime, TimeshiftConstants.TIME_SHIFT_END_TIME), TimeZoneInfo.Local.GetUtcOffset(currentDate)).LastDayOfMonth(true);
            return returnDate;
        }

        public static DateTimeOffset GetWeeklyExpiry(DateTimeOffset currentDate, bool isShift)
        {
            var returnDate = isShift
                ? new DateTimeOffset(GetDateWithNewTime(currentDate.DateTime, TimeshiftConstants.TIME_SHIFT_END_TIME), TimeZoneInfo.Local.GetUtcOffset(currentDate)).AddDays(7).LastDayOfWeek(true)
                : new DateTimeOffset(GetDateWithNewTime(currentDate.DateTime, TimeshiftConstants.TIME_SHIFT_END_TIME), TimeZoneInfo.Local.GetUtcOffset(currentDate)).LastDayOfWeek(true);
            return returnDate;
        }

        private static DateTime GetDateWithNewTime(DateTime currentDate, TimeSpan timeSpan)
        {
            return new DateTime(currentDate.Date.Year, currentDate.Month, currentDate.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        public static string CleanAnswer(string answer)
        {
            var cleanAnswer = answer
                .ToLower()
                .Replace("(", " ")
                .Replace(")", " ")
                .Replace("/", " ")
                .Replace(@"\", " ")
                .Replace(@".", " ")
                .Replace(@",", " ");

            cleanAnswer = Regex.Replace(cleanAnswer, @"\s+", " ").Trim();

            return cleanAnswer;
        }

        public static string ToPersonaFormat(this string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = CleanAnswer(text);

                var splitText = text.Split(' ');
                if (splitText.Length > 2)
                {
                    text = $"{splitText[0]} {splitText[1]}";
                }
            }
            return text;
        }
    }
}
