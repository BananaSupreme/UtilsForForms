using SkiaSharp;

using System.Linq;

namespace Xamarin.Forms.Utils
{
	public static class SKCanvasExtensions
	{
		public static void DrawRoundRectWithInnerShadow( this SKCanvas canvas, SKRoundRect bounds, SKColor innerRectColor, SKColor shadowColor, float shadowRatio = 0.025f )
		{
			using SKPaint paint = new()
			{
				Style = SKPaintStyle.Fill,
				IsAntialias = true,
				Color = shadowColor
			};

			float averagedRatio = ( bounds.Rect.Width * shadowRatio + bounds.Rect.Height * shadowRatio ) / 2;
			canvas.DrawRoundRect( bounds, paint );
			using SKRoundRect inner = new(
				new SKRect( bounds.Rect.Left + averagedRatio,
				bounds.Rect.Top + averagedRatio,
				bounds.Rect.Right - averagedRatio,
				bounds.Rect.Bottom - averagedRatio ), 0 );
			inner.SetRectRadii( inner.Rect, bounds.Radii );
			using SKPaint innerPaint = new()
			{
				Style = SKPaintStyle.Fill,
				Color = innerRectColor,
				IsAntialias = true,
				MaskFilter = SKMaskFilter.CreateBlur( SKBlurStyle.Normal, averagedRatio * 0.75f )
			};
			canvas.DrawRoundRect( inner, innerPaint );
		}
	}
}