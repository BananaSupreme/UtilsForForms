using System;
using System.Globalization;

namespace Xamarin.Forms.Utils.Converters
{
	public class NullableDoubleToStringConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value is double d )
			{
				return d.ToString();
			}
			else
			{
				return parameter is string s ? s : String.Empty;
			}
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value is string str )
				return Double.TryParse( str, out double outValue ) ? (double?)outValue : null;
			else
				return null;
		}
	}
}