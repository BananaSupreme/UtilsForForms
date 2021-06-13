using System.Threading.Tasks;

using Xamarin.Forms.Utils.Effects;

namespace Xamarin.Forms.Utils.Controls
{
	public class ScrollViewRTLFix : ScrollView
	{
		static readonly OverScrollDisabler s_overscrollEffect = new OverScrollDisabler();

		public static BindableProperty DisableOverscrollProperty = BindableProperty.Create(
			nameof( DisableOverscroll ), typeof( bool ), typeof( ScrollViewRTLFix ), true,
			propertyChanged: ( bindable, oldValue, newValue ) =>
			{
				if ( bindable is ScrollView control && newValue is bool value )
				{
					if ( value )
					{
						if ( !control.Effects.Contains( s_overscrollEffect ) )
							control.Effects.Add( s_overscrollEffect );
					}
					else
					{
						if ( control.Effects.Contains( s_overscrollEffect ) )
							_ = control.Effects.Remove( s_overscrollEffect );
					}
				}
			} );

		public bool DisableOverscroll
		{
			get => (bool)GetValue( DisableOverscrollProperty );
			set => SetValue( DisableOverscrollProperty, value );
		}

		public ScrollViewRTLFix() : base()
		{
			SizeChanged += ( s, e ) =>
			{
				if ( this is IVisualElementController controller && controller.EffectiveFlowDirection.IsRightToLeft() )
					_ = RTLAwareScrollToAsync( 0, 0, false );
				else
					return;
			};
			HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
			VerticalScrollBarVisibility = ScrollBarVisibility.Never;

			Effects.Add( s_overscrollEffect );
		}

		public Task RTLAwareScrollToAsync( double x, double y, bool animated )
		{
			if ( this is IVisualElementController controller && controller.EffectiveFlowDirection.IsRightToLeft() )
				return ScrollToAsync( ContentSize.Width - x, y, animated );
			else
				return ScrollToAsync( x, y, animated );
		}
	}
}