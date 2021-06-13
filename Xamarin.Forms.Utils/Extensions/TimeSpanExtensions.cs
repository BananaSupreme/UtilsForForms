using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Utils
{
	public static class TimeSpanExtensions
	{
		public static TimeSpan Time( this TimeSpan time )
		{
			return new TimeSpan( time.Days, time.Hours, time.Minutes, 0 );
		}
	}
}