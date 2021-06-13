using Xamarin.Forms.Utils.Droid.Effects;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;

[assembly: ResolutionGroupName( "Xamarin.Forms.Utils" )]
[assembly: ExportEffect( typeof( OverScrollDisablerEffect ), "OverScrollDisabler" )]

namespace Xamarin.Forms.Utils.Droid.Effects
{
	public class OverScrollDisablerEffect : PlatformEffect
	{
		protected override void OnAttached()
		{
			Control.OverScrollMode = OverScrollMode.Never;
		}

		protected override void OnDetached()
		{
		}
	}
}