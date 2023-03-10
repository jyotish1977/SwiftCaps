using System;
using System.Globalization;
using SwiftCaps.Models.Models;
using Xamarin.Forms;

namespace SwiftCaps.Converters
{
    public class MaximumIndicatorVisibleConverter : IValueConverter
    {
        private const int _maximumVisibleCount = 3;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is QuizSection quizSection)
            {
                var questionsCount = quizSection.Questions.Count;
                if (questionsCount > _maximumVisibleCount)
                {
                    return _maximumVisibleCount;
                }
                return questionsCount;
            }
            return 0;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
