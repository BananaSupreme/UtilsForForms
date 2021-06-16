using SkiaSharp.Views.Forms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Utils.Controls.Rotator
{
	[XamlCompilation( XamlCompilationOptions.Compile )]
	public partial class RotatorControl : ContentView
	{
		bool _running;
		bool _moved;
		bool _isAnimating;
		bool _isRtl;
		double _percent;
		double _totalX;
		double _startX;
		double _pixelWidth = -1;
		double _pixelHeight = -1;

		const double START_THRESHOLD = 0.05;
		const double END_THRESHOLD = 0.65;

		public static readonly BindableProperty LeftViewProperty = BindableProperty.Create(
			nameof( LeftView ), typeof( View ), typeof( RotatorControl ), null,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is RotatorControl control && newValue is View view )
				{
					if ( control._isAnimating )
						return;

					if ( control._isRtl )
					{
						control._views[2] = view;
					}
					else if ( !control._isRtl )
					{
						control._views[0] = view;
					}
				}
			} );

		public View LeftView
		{
			get => (View)GetValue( LeftViewProperty );
			set => SetValue( LeftViewProperty, value );
		}

		public static readonly BindableProperty CenterViewProperty = BindableProperty.Create(
			nameof( CenterView ), typeof( View ), typeof( RotatorControl ), null,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is RotatorControl control && newValue is View view )
				{
					if ( control._isAnimating )
						return;

					control._views[1] = view;
				}
			} );

		public View CenterView
		{
			get => (View)GetValue( CenterViewProperty );
			set => SetValue( CenterViewProperty, value );
		}

		public static readonly BindableProperty RightViewProperty = BindableProperty.Create(
			nameof( RightView ), typeof( View ), typeof( RotatorControl ), null,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is RotatorControl control && newValue is View view )
				{
					if ( control._isAnimating )
						return;

					if ( control._isRtl )
					{
						control._views[0] = view;
					}
					else if ( !control._isRtl )
					{
						control._views[2] = view;
					}
				}
			} );

		public View RightView
		{
			get => (View)GetValue( RightViewProperty );
			set => SetValue( RightViewProperty, value );
		}

		public static readonly BindableProperty RotatorControllerProperty = BindableProperty.Create(
			nameof( RotatorController ), typeof( IRotatorController ), typeof( RotatorControl ), null );

		public IRotatorController RotatorController
		{
			get => (IRotatorController)GetValue( RotatorControllerProperty );
			set => SetValue( RotatorControllerProperty, value );
		}

		public event EventHandler<RotatorShiftedEventArgs> RotatorShifted;

		View[] _views;

		public RotatorControl()
		{
			InitializeComponent();
			_views = new View[3];

			RotatorShifted += OnRotatorShift;
			SizeChanged += ( s, e ) => _isRtl = this.IsRtl();
			ResetTransformation();
		}

		async void OnCalanderPanned( object sender, SKTouchEventArgs e )
		{
			if ( _pixelWidth == -1 )
				return;

			double absX = Math.Abs( _totalX );
			double percent = Math.Min( Math.Max( absX / ( _pixelWidth * 0.6 ), 0 ), 1 );

			switch ( e.ActionType )
			{
				case SKTouchAction.Pressed:
					_totalX = 0;
					_startX = e.Location.X;
					break;

				case SKTouchAction.Moved:
					if ( !e.InContact )
						break;

					_totalX = e.Location.X - _startX;
					if ( percent < START_THRESHOLD && !_running )
						break;

					_percent = percent;
					_moved = true;
					_running = true;
					if ( _totalX > 0 )
					{
						RotatorController.StepCenterToRight( _views[1], percent );

						RotatorController.StepLeftToCenter( _views[0], percent );

						_views[2].ScaleX = 0;
					}
					else
					{
						_views[0].ScaleX = 0;

						RotatorController.StepCenterToLeft( _views[1], percent );

						RotatorController.StepRightToCenter( _views[2], percent );
					}
					break;

				case SKTouchAction.Released:
				case SKTouchAction.Cancelled:
					if ( !_moved )
					{
						double XTapPercent = e.Location.Y / _pixelHeight;
						double YTapPercent = e.Location.X / _pixelWidth;
						RotatorController.OnTapped( XTapPercent, YTapPercent );
						break;
					}
					_moved = false;
					if ( _views[1].ScaleX > END_THRESHOLD )
					{
						await UndoMove();
					}
					else if ( _views[1].AnchorX == 0 )
					{
						RotatorShifted?.Invoke( this, new( _isRtl ? RotatorShift.Backwords : RotatorShift.Forwards ) );
					}
					else
					{
						RotatorShifted?.Invoke( this, new( _isRtl ? RotatorShift.Forwards : RotatorShift.Backwords ) );
					}

					_running = false;
					break;
			}

			e.Handled = true;
		}

		async Task UndoMove()
		{
			await Task.WhenAll(
				RotatorController.MoveCenterToLeftAnimated( _views[0], _percent ),
				RotatorController.MoveRightToCenterAnimated( _views[1], _percent ),
				RotatorController.MoveLeftToCenterAnimated( _views[2], _percent ) );
			ResetTransformation();
		}

		void ResetTransformation()
		{
			_percent = 0;

			_views[0].InputTransparent = true;

			_views[1].InputTransparent = false;

			_views[2].InputTransparent = true;
		}

		async void OnRotatorShift( object sender, RotatorShiftedEventArgs e )
		{
			_isAnimating = true;

			int shift = e.rotatorShift == RotatorShift.Forwards ? 1 : -1;

			shift = _isRtl ? shift : -shift;

			if ( shift == 1 )
			{
				_views = _views.Rotate( 2 ).ToArray();
				_views[0].Opacity = 0;
				await Task.WhenAll(
						RotatorController.MoveCenterToRightAnimated( _views[2], _percent ),
						RotatorController.MoveLeftToCenterAnimated( _views[1], _percent ) );
			}
			else if ( shift == -1 )
			{
				_views = _views.Rotate( 1 ).ToArray();
				_views[2].Opacity = 0;
				await Task.WhenAll(
						RotatorController.MoveCenterToLeftAnimated( _views[0], _percent ),
						RotatorController.MoveRightToCenterAnimated( _views[1], _percent ) );
			}

			_isAnimating = false;
			ResetTransformation();
		}

		void OnPaint( object sender, SKPaintSurfaceEventArgs e )
		{
			_pixelWidth = e.Info.Width;
			_pixelHeight = e.Info.Height;
		}
	}
}