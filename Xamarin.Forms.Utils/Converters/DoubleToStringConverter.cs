using System;
using System.Globalization;

using Xamarin.Forms;

namespace Xamarin.Forms.Utils.Converters
{
	internal class DoubleToStringConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			double d = (double)value;
			return d == 0 ? String.Empty : d.ToString();
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return Double.TryParse( (string)value, out double d ) ? d : (object)0d;
		}
	}
}