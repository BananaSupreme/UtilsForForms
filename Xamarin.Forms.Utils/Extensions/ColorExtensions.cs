using System;

using Xamarin.Forms;

namespace Xamarin.Forms.Utils
{
	public static class ColorExtensions
	{
		public static Color Mix( this Color self, Color mixed, float proportion = 0.5f )
		{
			proportion = Math.Min( 1, proportion );
			return new Color
				(
					r: self.R + proportion * ( mixed.R - self.R ),
					g: self.G + proportion * ( mixed.G - self.G ),
					b: self.B + proportion * ( mixed.B - self.B ),
					a: self.A + proportion * ( mixed.A - self.A )
				);
		}
	}
}