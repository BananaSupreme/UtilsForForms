using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Utils.Controls
{
	[XamlCompilation( XamlCompilationOptions.Compile )]
	public partial class Divider : ContentView
	{
		public static readonly BindableProperty ProportionalOffsetProperty = BindableProperty.Create(
			"ProportionalOffset", typeof( float ), typeof( Divider ), 0.2f );

		public float ProportionalOffset
		{
			get => (float)GetValue( ProportionalOffsetProperty );
			set => SetValue( ProportionalOffsetProperty, value );
		}

		public static readonly BindableProperty RadiusToLengthRatioProperty = BindableProperty.Create(
			"RadiusToLengthRatio", typeof( float ), typeof( Divider ), 0.62f );

		public float RadiusToLengthRatio
		{
			get => (float)GetValue( RadiusToLengthRatioProperty );
			set => SetValue( RadiusToLengthRatioProperty, value );
		}

		public static readonly BindableProperty VerticalProperty = BindableProperty.Create(
			"IsVertical", typeof( bool ), typeof( Divider ), false );

		public bool IsVertical
		{
			get => (bool)GetValue( VerticalProperty );
			set => SetValue( VerticalProperty, value );
		}

		public Divider()
		{
			InitializeComponent();
		}

		void OnPaintSurface( object sender, SKPaintSurfaceEventArgs e )
		{
			using SKCanvas canvas = e.Surface.Canvas;
			SKImageInfo info = e.Info;
			canvas.Clear();

			Color dividerColor = (Color)Application.Current.Resources[ColorStrings.Secondry];
			SKColor sparkle = dividerColor.Mix( (Color)Application.Current.Resources[ColorStrings.Main], 0.5f ).Mix( Color.Silver, 0.35f ).ToSKColor();
			float length = IsVertical ? info.Height : info.Width;
			float ratio = IsVertical ? (float)Height / info.Height : (float)Width / info.Width;
			float offset = ProportionalOffset * length;

			SKPoint centre = IsVertical ? new SKPoint( 0.5f, offset ) : new SKPoint( offset, 0.5f );
			float radius = length * RadiusToLengthRatio;

			using SKPaint dividerPaint = new SKPaint
			{
				Shader = SKShader.CreateRadialGradient(
					centre,
					radius,
					new SKColor[] { sparkle, dividerColor.ToSKColor(), dividerColor.MultiplyAlpha( 0 ).ToSKColor() },
					new float[] { 0, 0.1f, 1f },
					SKShaderTileMode.Clamp )
			};
			{
				if ( !IsVertical )
				{
					canvas.DrawRect( 0, 0, info.Width, 1f / ratio, dividerPaint );
				}
				else
				{
					canvas.DrawRect( 0, 0, 1f / ratio, info.Height, dividerPaint );
				}
			}
		}
	}
}