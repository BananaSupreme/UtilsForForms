using System;

namespace Xamarin.Forms.Utils
{
	public static class StringExtensions
	{
		public static string Reverse( this string str )
		{
			char[] chars = str.ToCharArray();
			Array.Reverse( chars );
			return new string( chars );
		}
	}
}