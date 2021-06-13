using System;
using System.Globalization;

using Xamarin.Forms;

namespace Xamarin.Forms.Utils.Converters
{
	internal class BoolColourConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value != null
				 && parameter is Color color )
			{
				Color CustomColour = color;
				return (bool)value ? CustomColour : CustomColour.MultiplyAlpha( 0.5 );
			}
			else
				return default;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}