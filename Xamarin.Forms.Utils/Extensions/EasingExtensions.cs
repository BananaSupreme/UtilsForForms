using System;

using Xamarin.Forms;

namespace Xamarin.Forms.Utils
{
	public class EasingExtensions
	{
		public static readonly Easing Exponential = new Easing( x => x == 1 ? 1 : Math.Pow( 2f, 10f * x - 10f ) );
		public static readonly Easing Logarithmic = new Easing( x => x == 1 ? 1 : 1f - Math.Pow( 2f, -10f * x ) );

		public static readonly Easing ElasticInOut = new Easing( x =>
		 x == 0
		 ? 0
		 : x == 1
		 ? 1
		 : x < 0.5
		 ? -( Math.Pow( 2f, 20f * x - 10f ) * Math.Sin( ( 20f * x - 11.125f ) * ( 2f * Math.PI ) / 4.5f ) ) / 2f
		 : Math.Pow( 2f, -20f * x + 10f ) * Math.Sin( ( 20f * x - 11.125f ) * ( 2f * Math.PI ) / 4.5f ) / 2f + 1f );
	}
}