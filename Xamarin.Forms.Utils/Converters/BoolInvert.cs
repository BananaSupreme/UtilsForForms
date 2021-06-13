using System;
using System.Globalization;

namespace Xamarin.Forms.Utils.Converters
{
	internal class BoolInvert : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return !(bool)value;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return !(bool)value;
		}
	}
}