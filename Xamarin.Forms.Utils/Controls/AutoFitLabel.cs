using System;

namespace Xamarin.Forms.Utils.Controls
{
	public class AutoFitLabel : Label
	{
		public static readonly BindableProperty MaxFontSizeProperty = BindableProperty.Create(
			nameof( MaxFontSize ), typeof( double ), typeof( AutoFitLabel ), 0d, defaultValueCreator:
			( sender ) =>
			{
				if ( sender is Label control )
				{
					return control.FontSize;
				}
				else
					return default;
			} );

		[TypeConverter( typeof( FontSizeConverter ) )]
		public double MaxFontSize
		{
			get => (double)GetValue( MaxFontSizeProperty );
			set => SetValue( MaxFontSizeProperty, value );
		}

		protected override void OnSizeAllocated( double width, double height )
		{
			base.OnSizeAllocated( width, height );
			if ( width < 0 || height < 0 )
				return;
			FontSize = MaxFontSize;

			double desiredWidth = LineBreakMode == LineBreakMode.NoWrap ? double.PositiveInfinity : width;
			double desiredHeight = LineBreakMode == LineBreakMode.NoWrap ? height : double.PositiveInfinity;
			SizeRequest desiredRequest = Measure( desiredWidth, desiredHeight );
			Size desired = desiredRequest.Request;

			if ( desired.Width > width || desired.Height > height )
			{
				FontSize *= Math.Min( width / desired.Width, height / desired.Height );
			}
		}
	}
}