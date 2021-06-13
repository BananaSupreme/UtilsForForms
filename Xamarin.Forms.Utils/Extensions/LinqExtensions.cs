using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.Utils
{
	public static class LinqExtensions
	{
		public static IEnumerable<T> Rotate<T>( this IEnumerable<T> e, int n ) =>
		n >= 0 ? e.Skip( n ).Concat( e.Take( n ) ) : e.Reverse().Skip( n ).Concat( e.Take( n ) ).Reverse();
	}
}