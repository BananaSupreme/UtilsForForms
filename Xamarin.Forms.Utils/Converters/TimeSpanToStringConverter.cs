using System;
using System.Globalization;

using Xamarin.Forms;

namespace Celery.Converters
{
	public class TimeSpanToStringConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value is TimeSpan time )
				return time.ToString( "hh\\:mm" );
			else if ( parameter is string s )
			{
				return s;
			}
			else
				return String.Empty;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value is string time )
			{
				if ( TimeSpan.TryParse( time, out TimeSpan returnedTime ) )
					return returnedTime;
				else
				{
					try
					{
						string[] splitTime = time.Split( ':' );
						return new TimeSpan( Int32.Parse( splitTime[0] ), Int32.Parse( splitTime[1] ), 0 );
					}
					catch ( FormatException )
					{
						return new TimeSpan();
					}
				}
			}
			else
				return new TimeSpan();
		}
	}
}