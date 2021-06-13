using SkiaSharp;
using SkiaSharp.Views.Forms;

using System;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Utils.Controls
{
	[XamlCompilation( XamlCompilationOptions.Compile )]
	public partial class MaterialEntry : ContentView
	{
		private float _borderHeight;
		private float _borderWidth;
		private float _controlHeight;
		private float _cornerRadius;
		private double _density;
		private float _empty;
		private float _pixelHeight;
		private float _pixelWidth;
		private bool _tapped;

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
			nameof( CommandParamter ), typeof( object ), typeof( MaterialEntry ), null, BindingMode.OneWay );

		public static readonly BindableProperty CommandProperty = BindableProperty.Create(
			nameof( Command ), typeof( ICommand ), typeof( MaterialEntry ), null, BindingMode.OneWay );

		public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(
			"ErrorText", typeof( string ), typeof( MaterialEntry ), String.Empty );

		public static readonly BindableProperty HelperTextProperty = BindableProperty.Create(
			"HelperText", typeof( string ), typeof( MaterialEntry ), String.Empty );

		public static readonly BindableProperty IsErrorProperty = BindableProperty.Create(
			"IsError", typeof( bool ), typeof( MaterialEntry ), false
			, propertyChanged: ( bindable, oldValue, newValue ) =>
			 {
				 if ( bindable is MaterialEntry control && newValue != null && newValue != oldValue )
					 control.control.InvalidateSurface();
			 } );

		public static readonly BindableProperty IsNumericProperty = BindableProperty.Create(
			"IsNumeric", typeof( bool ), typeof( MaterialEntry ), false
			, propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is MaterialEntry control && newValue != null && newValue != oldValue )
				{
					control.entry.Keyboard = (bool)newValue ? Keyboard.Numeric : Keyboard.Text;
					control.label.FontFamily = (bool)newValue ?
						(OnPlatform<string>)Application.Current.Resources["RobotoRegular"] :
						(OnPlatform<string>)Application.Current.Resources["GeneralHebrewText"];
				}
			} );

		public static readonly BindableProperty OverrideEntryProperty = BindableProperty.Create(
			"OverrideEntry", typeof( bool ), typeof( MaterialEntry ), false,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is MaterialEntry control && newValue != null && oldValue != newValue )
				{
					/*bool value = (bool)newValue;
                    control.entry.InputTransparent = value;
                    Device.BeginInvokeOnMainThread(() => control.tap.IsVisible = value);*/
				}
			} );

		public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
			"PlaceholderText", typeof( string ), typeof( MaterialEntry ), String.Empty );

		public static readonly BindableProperty TextProperty = BindableProperty.Create(
																			"Text", typeof( string ), typeof( MaterialEntry ), String.Empty, BindingMode.TwoWay,
			propertyChanged: OnTextChanged );

		public ICommand Command
		{
			get { return (ICommand)GetValue( CommandProperty ); }
			set { SetValue( CommandProperty, value ); }
		}

		public object CommandParamter
		{
			get { return (object)GetValue( CommandParameterProperty ); }
			set { SetValue( CommandParameterProperty, value ); }
		}

		public string ErrorText
		{
			get => (string)GetValue( ErrorTextProperty );
			set => SetValue( ErrorTextProperty, value );
		}

		public string HelperText
		{
			get => (string)GetValue( HelperTextProperty );
			set => SetValue( HelperTextProperty, value );
		}

		public bool IsError
		{
			get => (bool)GetValue( IsErrorProperty );
			set => SetValue( IsErrorProperty, value );
		}

		public bool IsNumeric
		{
			get => (bool)GetValue( IsNumericProperty );
			set => SetValue( IsNumericProperty, value );
		}

		public bool OverrideEntry
		{
			get => (bool)GetValue( OverrideEntryProperty );
			set => SetValue( OverrideEntryProperty, value );
		}

		public string PlaceholderText
		{
			get => (string)GetValue( PlaceholderTextProperty );
			set => SetValue( PlaceholderTextProperty, value );
		}

		public string Text
		{
			get => (string)GetValue( TextProperty );
			set => SetValue( TextProperty, value );
		}

		public static event EventHandler<TextChangedEventArgs> TextChanged;

		public event EventHandler EntryFocused;

		public event EventHandler EntryUnfocused;

		public event EventHandler Tapped;

		public MaterialEntry()
		{
			InitializeComponent();

			entry.Focused += ( s, e ) => EntryFocused?.Invoke( this, EventArgs.Empty );
			entry.Unfocused += ( s, e ) => EntryUnfocused?.Invoke( this, EventArgs.Empty );
		}

		private static void OnTextChanged( object bindable, object oldValue, object newValue )
		{
			if ( oldValue is string oldString
				&& newValue is string newString
				&& bindable is MaterialEntry control )
			{
				TextChanged?.Invoke( control, new TextChangedEventArgs( oldString, newString ) );
				control.label.Text = newString;
				if ( control.entry.Text != newString )
					control.entry.Text = newString;
				control.control.InvalidateSurface();
			}
		}

		private void OnControlPaintSurface( object sender, SKPaintSurfaceEventArgs e )
		{
			using SKCanvas canvas = e.Surface.Canvas;
			SKImageInfo info = e.Info;
			canvas.Clear();

			Color primary = (Color)Application.Current.Resources[ColorStrings.Main];
			Color secondry = (Color)Application.Current.Resources[ColorStrings.Secondry];
			Color flyout = (Color)Application.Current.Resources[ColorStrings.SecondryBackground];
			Color background = (Color)Application.Current.Resources[ColorStrings.Background];
			Color error = (Color)Application.Current.Resources[ColorStrings.Warning];

			_density = Height / info.Height;
			float standardMargins = ( 6f / (float)_density );

			SKTypeface typeface = FontManager.GetFont( FontStrings.GeneralHebrewText );
			float textSize = (float)( 16f / _density );

			bool isRtl = this.IsRtl();
			bool isUnderTextExist =
				IsError && !String.IsNullOrWhiteSpace( ErrorText )
				|| !String.IsNullOrWhiteSpace( HelperText );

			label.TextColor =
				IsError
				? error
				: entry.IsFocused || !String.IsNullOrWhiteSpace( Text )
				? primary.MultiplyAlpha( 0.8 )
				: primary;

			SKRect underTextBounds = new SKRect( 0, 0, 0, 0 );
			string underText = IsError ? ErrorText : HelperText;
			Color underTextColor = IsError ? error : secondry.MultiplyAlpha( 0.6 );
			using SKPaint underTextPaint = new SKPaint
			{
				Color = underTextColor.ToSKColor(),
				Typeface = typeface,
				TextSize = textSize
			};

			underTextPaint.MeasureText(
				!String.IsNullOrWhiteSpace( underText )
				? underText : "A"
				, ref underTextBounds );

			_borderWidth = info.Width - ( 8f / (float)_density );
			_borderHeight = (float)Math.Min( info.Height - standardMargins * 3 - underTextBounds.Height * 1.5
											, _borderWidth * 0.225 );
			_controlHeight = (float)( _borderHeight + 3 * standardMargins + underTextBounds.Height * 1.5 );
			_empty = ( info.Height - _controlHeight ) * 0.5f;
			_cornerRadius = _borderHeight * 0.5f;
			_pixelHeight = info.Height;
			_pixelWidth = info.Width;

			using SKRoundRect border = new SKRoundRect(
				isUnderTextExist
				? new SKRect( standardMargins
					  , _empty + 0.5f * underTextBounds.Height
					  , _pixelWidth - standardMargins
					  , _empty + 0.5f * underTextBounds.Height + _borderHeight )
				: new SKRect( standardMargins
					  , ( _pixelHeight - _borderHeight ) / 2
					  , _pixelWidth - standardMargins
					  , ( ( _pixelHeight - _borderHeight ) / 2 ) + _borderHeight )
				, _cornerRadius );

			using SKPaint borderFill = new SKPaint
			{
				Color = flyout.ToSKColor()
			};
			canvas.DrawRoundRect( border, borderFill );

			bool entryActive = entry.IsFocused
				&& !OverrideEntry
				&& ( _tapped );

			if ( entryActive || !String.IsNullOrWhiteSpace( Text ) )
			{
				if ( label.Text == PlaceholderText )
					label.Text = String.Empty;

				using SKPaint borderPaint = new SKPaint
				{
					Color = secondry.MultiplyAlpha( entryActive ? 1 : 0.6 ).ToSKColor(),
					Style = SKPaintStyle.Stroke,
					StrokeWidth = 6
				};
				canvas.DrawRoundRect( border, borderPaint );

				using SKPaint placeholderTextPaint = new SKPaint
				{
					Color = secondry.MultiplyAlpha( entryActive ? 1 : 0.6 ).ToSKColor(),
					Typeface = typeface,
					TextSize = textSize,
					FakeBoldText = true
				};

				SKRect placeholderTextBounds = new SKRect();
				placeholderTextPaint.MeasureText( PlaceholderText, ref placeholderTextBounds );

				SKRect borderlessSection = isRtl ?
					new SKRect( _pixelWidth - _cornerRadius - placeholderTextBounds.Width - standardMargins * 2,
					border.Rect.Top - borderPaint.StrokeWidth / 2,
					_pixelWidth - _cornerRadius,
					border.Rect.Top + borderPaint.StrokeWidth / 2 )
					: new SKRect( _cornerRadius + standardMargins * 2,
					border.Rect.Top - borderPaint.StrokeWidth / 2,
					_cornerRadius + standardMargins * 4 + placeholderTextBounds.Width,
					border.Rect.Top + borderPaint.StrokeWidth / 2 );
				using SKPaint borderlessSectionPaint = new SKPaint
				{
					Shader = SKShader.CreateLinearGradient(
						new SKPoint( borderlessSection.MidX, borderlessSection.Bottom ),
						new SKPoint( borderlessSection.MidX, borderlessSection.Top ),
						new SKColor[] { flyout.ToSKColor(), background.ToSKColor() },
						SKShaderTileMode.Decal
						)
				};
				canvas.DrawRect( borderlessSection, borderlessSectionPaint );
				canvas.DrawText( isRtl ? PlaceholderText.Reverse() : PlaceholderText,
					borderlessSection.Left + standardMargins,
					border.Rect.Top + ( placeholderTextBounds.Height / 2 ),
					placeholderTextPaint );
			}
			else
			{
				label.Text = PlaceholderText;
				using SKPaint borderStroke = new SKPaint
				{
					Color = secondry.MultiplyAlpha( 0.25 ).ToSKColor(),
					Style = SKPaintStyle.Stroke,
					StrokeWidth = 6
				};

				canvas.DrawRoundRect( border, borderStroke );
			}

			if ( isUnderTextExist )
			{
				float orginalTextHeight = underTextBounds.Height;
				if ( underTextBounds.Width > ( _borderWidth - _cornerRadius * 2 ) * 0.85 )
				{
					underTextPaint.TextSize *= (float)( ( _borderWidth - _cornerRadius * 2 ) * 0.82 / underTextBounds.Width );
					underTextPaint.MeasureText( underText, ref underTextBounds );
				}

				canvas.DrawText( isRtl ? underText.Reverse() : underText
							   , isRtl ? _pixelWidth - underTextBounds.Width - _cornerRadius - standardMargins
							   : _cornerRadius - standardMargins
							   , (float)( _pixelHeight - _empty - orginalTextHeight )
							   , underTextPaint );
			}

			label.Bounds.Center.Deconstruct( out double labelX, out double labelY );
			label.TranslationX = ( control.X + border.Rect.MidX * _density ) - labelX;
			label.TranslationY = ( control.Y + border.Rect.MidY * _density ) - labelY;
			entry.Bounds.Center.Deconstruct( out double entryX, out double entryY );
			entry.TranslationX = ( control.X + border.Rect.MidX * _density ) - entryX;
			entry.TranslationY = ( control.Y + border.Rect.MidY * _density ) - entryY;
			ResizeLabel();
		}

		private void OnControlTapped( object sender, EventArgs e )
		{
			if ( !OverrideEntry )
			{
				_tapped = true;
				entry.Focus();
			}
			else
			{
				Tapped?.Invoke( this, new EventArgs() );
				Command.SafeExecute( CommandParamter );
			}
		}

		private void OnEntryTextChanged( object sender, TextChangedEventArgs e )
		{
			Text = e.NewTextValue;
		}

		private void ResizeLabel()
		{
			SKTypeface typeface = IsNumeric ? FontManager.GetFont( FontStrings.GeneralNumbers ) : FontManager.GetFont( FontStrings.GeneralHebrewText );
			using SKPaint textMeasure = new SKPaint
			{
				Typeface = typeface,
				TextSize = (float)( _borderHeight * 0.5 )
			};
			SKRect bounds = new SKRect();
			textMeasure.MeasureText( label.Text, ref bounds );

			if ( bounds.Width > _borderWidth - _borderHeight * 1.5f )
				textMeasure.TextSize *= ( _borderWidth - _borderHeight * 1.5f ) / bounds.Width;
			label.FontSize = textMeasure.TextSize * _density;
			entry.FontSize = label.FontSize;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			control.InvalidateSurface();
		}
	}
}