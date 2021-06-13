using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Utils
{
	public static class StringBuilderExtensions
	{
		public static void AppendCsv( this StringBuilder builder, string data, bool last = false )
		{
			builder.Append( data );
			if ( !last )
				builder.Append( "," );
		}
	}
}