using SkiaSharp;
using SkiaSharp.Views.Forms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Utils.Controls.WheelPickerControl
{
	[XamlCompilation( XamlCompilationOptions.Compile )]
	public partial class WheelPicker : ContentView, INotifyPropertyChanged
	{
		IList<string> _baseItemList;
		ObservableRangeCollection<WheelValue> _observedItemList;

		public ObservableRangeCollection<WheelValue> ObservedItemList
		{
			get => _observedItemList;
			set
			{
				if ( _observedItemList != value )
				{
					_observedItemList = value;
					RaisePropertyChanged( nameof( ObservedItemList ) );
				}
			}
		}

		bool _updatingList;

		public bool UpdatingList
		{
			get => _updatingList;
			private set => _updatingList = value;
		}

		bool _firstLoad;

		double _itemHeight;
		double _accumelatedOffset;
		int _itemsViewed = 7;
		int _selectedItemIndex = 3;
		private int HalfItemsViewed { get => (int)Math.Floor( _itemsViewed / 2d ); }

		new public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler ListChanged;

		public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

		public static readonly BindableProperty MinProperty = BindableProperty.Create(
			"Min", typeof( int ), typeof( WheelPicker ), 0, BindingMode.OneWay,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is WheelPicker control && newValue != null )
				{
					control.CoerceValue( MaxProperty );
					if ( control.Mode == WheelPickerMode.Numeric )
					{
						control.ChangeList();
					}
				}
			} );

		public static readonly BindableProperty MaxProperty = BindableProperty.Create(
			"Max", typeof( int ), typeof( WheelPicker ), 1, BindingMode.OneWay,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is WheelPicker control && newValue != null )
				{
					control.CoerceValue( MaxProperty );
					if ( control.Mode == WheelPickerMode.Numeric )
						control.ChangeList();
				}
			},
			coerceValue: ( bindable, value ) =>
			{
				return bindable is WheelPicker control && value is int max
				? max > control.Min ? max
				: control.Min + 1 : (object)default;
			} );

		public static readonly BindableProperty ModeProperty = BindableProperty.Create(
			"Mode", typeof( WheelPickerMode ), typeof( WheelPicker ), WheelPickerMode.Numeric, BindingMode.OneWay );

		public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
			"FontSize", typeof( double ), typeof( WheelPicker ), 28d, BindingMode.OneWay );

		public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(
			"ItemSource", typeof( IEnumerable<string> ), typeof( WheelPicker ), default( IEnumerable<string> ), BindingMode.OneWay,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is WheelPicker control && newValue != null )
				{
					if ( control.Mode == WheelPickerMode.List )
						control.ChangeList();
				}
			} );

		public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
			"SelectedItem", typeof( string ), typeof( WheelPicker ), string.Empty, BindingMode.TwoWay,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is WheelPicker control && oldValue != newValue && newValue is string item )
				{
					control.SelectedItemChanged?.Invoke( control, new SelectedItemChangedEventArgs( item, control._baseItemList.IndexOf( item ) - 3 ) );
				}
			} );

		public int Min
		{
			get => (int)GetValue( MinProperty );
			set => SetValue( MinProperty, value );
		}

		public int Max
		{
			get => (int)GetValue( MaxProperty );
			set => SetValue( MaxProperty, value );
		}

		public double FontSize
		{
			get => (double)GetValue( FontSizeProperty );
			set => SetValue( FontSizeProperty, value );
		}

		public WheelPickerMode Mode
		{
			get => (WheelPickerMode)GetValue( ModeProperty );
			set => SetValue( ModeProperty, value );
		}

		public IEnumerable<string> ItemSource
		{
			get => (IEnumerable<string>)GetValue( ItemSourceProperty );
			set => SetValue( ItemSourceProperty, value );
		}

		public string SelectedItem
		{
			get => (string)GetValue( SelectedItemProperty );
			set => SetValue( SelectedItemProperty, value );
		}

		double _boxHeight;

		public double BoxHeight
		{
			get => _boxHeight;
			set
			{
				if ( _boxHeight != value )
				{
					_boxHeight = value;
					RaisePropertyChanged( nameof( BoxHeight ) );
				}
			}
		}

		public WheelPicker()
		{
			InitializeComponent();

			ObservedItemList = new ObservableRangeCollection<WheelValue>();

			BindingContext = this;
			DisplayedList.SetBinding( ItemsView.ItemsSourceProperty, "ObservedItemList", mode: BindingMode.OneWay );
		}

		//Loads selected item
		public void InitializeScroll()
		{
			ScrollTo( _baseItemList.IndexOf( SelectedItem ) + HalfItemsViewed, true );
			if ( _baseItemList.IndexOf( SelectedItem ) == 3 )
				_firstLoad = true;
		}

		public void ScrollToOffset( int offset, bool animate = true )
		{
			if ( ( SelectedItem == _baseItemList[0]
				&& offset < 0 ) || ( SelectedItem == _baseItemList[_baseItemList.Count - 1]
				&& offset > 0 ) )
			{
				return;
			}

			int index =
				offset < 0
				? _selectedItemIndex + offset - HalfItemsViewed
				: _selectedItemIndex + offset + HalfItemsViewed;
			ScrollTo( index: index, animate: animate );
		}

		public void ScrollTo( int index, bool animate = true )
		{
			_ = DisplayedList.Focus();
			DisplayedList.ScrollTo( index: index, animate: animate );
		}

		//Checks if fits the number of items allocated to screen, if not resizes proportionally.
		protected override void OnSizeAllocated( double width, double height )
		{
			base.OnSizeAllocated( width, height );

			if ( Application.Current.MainPage == null )
			{
				return;
			}

			BoxHeight = Math.Floor( height / _itemsViewed );

			Label ruler = CreateRuler();

			SizeRequest rulerSize = ruler.Measure( width, BoxHeight );
			double rulerHeight = rulerSize.Minimum.Height;

			if ( rulerHeight > BoxHeight )
			{
				_itemsViewed = (int)Math.Floor( height / rulerHeight );
				if ( _itemsViewed % 2 == 0 )
					_itemsViewed--;
				BoxHeight = Math.Floor( height / _itemsViewed );
				ruler = CreateRuler();
				rulerSize = ruler.Measure( width, BoxHeight );
				rulerHeight = rulerSize.Minimum.Height;
			}

			_itemHeight = rulerHeight;

			ChangeList();

			Label CreateRuler() => new Label
			{
				Text = "22222222",
				FontSize = FontSize,
				FontFamily = Application.Current.Resources["RobotoBold"].ToString(),
				HeightRequest = BoxHeight,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};
		}

		protected void ChangeList()
		{
			_updatingList = true;

			GenerateBaseItemList();

			ConvertBaseToObserved();

			if ( Mode == WheelPickerMode.Numeric )
			{
				int selectedItemIndex =
				_baseItemList.Contains( SelectedItem )
				? _baseItemList.IndexOf( SelectedItem )
				: Int32.Parse( SelectedItem ) > Max
				? Max : Min;
				int startIndex = _firstLoad ? selectedItemIndex - HalfItemsViewed : 0;

				ResizeList( startIndex );

				double offsetDiff = 0;

				if ( _firstLoad && !_baseItemList.Contains( SelectedItem ) )
				{
					int itemIndex = int.Parse( SelectedItem );
					offsetDiff =
						 itemIndex > Max
						? ( itemIndex - Max ) * _itemHeight
						: ( Min - itemIndex ) * _itemHeight;
				}

				if ( offsetDiff > 200 )
					ScrollTo( 0 );
				_accumelatedOffset = offsetDiff > 200 ? 0 : _accumelatedOffset - offsetDiff;
			}
			else
			{
				ResizeList( 0 );
				if ( _firstLoad )
					ScrollTo( 0 );
			}

			_updatingList = false;
		}

		private void ResizeList( int index )
		{
			double relativeItemOffset = ( _accumelatedOffset / _itemHeight ) - index;

			for ( int i = -1; i < _itemsViewed + 1; i++ )
			{
				double sizeOffsetSign = i - HalfItemsViewed <= 0 ? -1 : 1;
				double sizeOffset = Math.Min( relativeItemOffset * sizeOffsetSign * 2, 2 );
				ObservedItemList[( i + index ).Mod( ObservedItemList.Count )].Size = 28 - Math.Abs( ( i - HalfItemsViewed ) * 2 ) + sizeOffset;
			}
		}

		void ConvertBaseToObserved()
		{
			ObservedItemList.ReplaceRange
				   (
					_baseItemList
					.Select( m => new WheelValue( m, FontSize ) )
					.ToList()
				   );
		}

		private void GenerateBaseItemList()
		{
			int numebersBetweenMinMax = 1;
			if ( Mode == WheelPickerMode.Numeric && Min < Max )
			{
				numebersBetweenMinMax =
					Min < 0 && Max < 0
					? Math.Abs( Min ) - Math.Abs( Max ) + 1
					: Min < 0
					? 0 - Min + Max + 1
					: Max - Min + 1;
			}
			IList<string> preItemList = Mode switch
			{
				WheelPickerMode.List => ItemSource.ToList(),
				WheelPickerMode.Numeric => Enumerable.Range( Min, numebersBetweenMinMax )
				.Select( i => i.ToString( "D2" ) ).ToList(),
				_ => default
			};

			for ( int i = 0; i < 3; i++ )
			{
				preItemList.Insert( 0, String.Empty );
				preItemList.Insert( preItemList.Count, String.Empty );
			}

			_baseItemList = new List<string>( preItemList );
		}

		void OnGradiantPaint( object sender, SKPaintSurfaceEventArgs e )
		{
			using SKCanvas canvas = e.Surface.Canvas;
			SKImageInfo info = e.Info;

			float height = info.Height;
			float midX = info.Rect.MidX;
			//Define Background Colour here!
			Color Background = Color.Black;
			Color Transparent = Background.MultiplyAlpha( 0.25 );
			canvas.Clear();

			SKRect topRect = new SKRect( 0, 0, info.Width, height * ( 3f / 7f ) );
			SKRect bottomRect = new SKRect( 0, height * ( 4f / 7f ), info.Width, height );

			using SKPaint topGradiant = new SKPaint
			{
				Shader = SKShader.CreateLinearGradient(
					new SKPoint( midX, 0 ),
					new SKPoint( midX, topRect.Bottom ),
					new SKColor[] { Background.ToSKColor(), Transparent.ToSKColor() },
					SKShaderTileMode.Clamp
					)
			};
			using SKPaint bottomGradiant = new SKPaint
			{
				Shader = SKShader.CreateLinearGradient(
					new SKPoint( midX, bottomRect.Top ),
					new SKPoint( midX, height ),
					new SKColor[] { Transparent.ToSKColor(), Background.ToSKColor() },
					SKShaderTileMode.Clamp )
			};
			canvas.DrawRect( bottomRect, bottomGradiant );
			canvas.DrawRect( topRect, topGradiant );
		}

		protected void OnScrolled( object sender, ItemsViewScrolledEventArgs e )
		{
			if ( _updatingList )
				return;
			int firstVisible = e.FirstVisibleItemIndex;
			_accumelatedOffset += e.VerticalDelta;

			ResizeList( firstVisible );

			if ( _firstLoad )
			{
				_selectedItemIndex = e.CenterItemIndex;
				SelectedItem = ObservedItemList[_selectedItemIndex].Text;
			}
			else if ( e.CenterItemIndex >= _baseItemList.IndexOf( SelectedItem ) )
			{
				_firstLoad = true;
			}
			else
			{
				InitializeScroll();
			}
		}

		public void RaisePropertyChanged( params string[] propertyNames )
		{
			foreach ( string propertyName in propertyNames )
			{
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}
	}
}