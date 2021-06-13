using SkiaSharp;

namespace Xamarin.Forms.Utils
{
	public static class SKPointExtensions
	{
		public static bool In( this SKPoint point, SKRect rect )
		{
			return point.X > rect.Left && point.X < rect.Right
				&& point.Y > rect.Top && point.Y < rect.Bottom;
		}
	}
}