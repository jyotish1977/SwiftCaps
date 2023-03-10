using System;
using System.Globalization;
using SwiftCaps.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace SwiftCaps.Converters
{
    public class LeaderBoardQuizStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null)
                return ImageSource.FromResource("SwiftCaps.Resources.Images.redcircle.png");

            if (value is bool && (bool)value)
			{
                return ImageSource.FromResource("SwiftCaps.Resources.Images.greencircle.png"); 
            }

            return ImageSource.FromResource("SwiftCaps.Resources.Images.redcircle.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
