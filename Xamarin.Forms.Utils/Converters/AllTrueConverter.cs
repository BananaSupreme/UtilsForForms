using System;
using System.Globalization;

using Xamarin.Forms;

namespace Xamarin.Forms.Utils.Converters
{
	public class AllTrueConverter : IMultiValueConverter
	{
		public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
		{
			foreach ( object value in values )
			{
				if ( values == null || !targetType.IsAssignableFrom( typeof( bool ) ) )
					return false;
				if ( value is not bool b )
					return false;
				else if ( !b )
					return false;
			}
			return true;
		}

		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
		{
			return null;
		}
	}
}