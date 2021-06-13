using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Utils
{
	public static class DateTimeExtensions
	{
		public static DateTime MonthFirst( this DateTime date )
		{
			return new DateTime( date.Year, date.Month, 1 );
		}

		public static DateTime MonthLast( this DateTime date )
		{
			return new DateTime( date.Year, date.Month, DateTime.DaysInMonth( date.Year, date.Month ) );
		}

		public static int MonthsBetween( this DateTime startDate, DateTime endDate )
		{
			return ( endDate.Year - startDate.Year ) * 12 + ( endDate.Month - startDate.Month );
		}
	}
}