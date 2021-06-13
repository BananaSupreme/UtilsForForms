using System;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;

namespace Xamarin.Forms.Utils.Services
{
	public class DialogService : IDialogService
	{
		public static IDialogService Instance = new DialogService();

		public static async Task<bool> RaiseMessageAsync( string message, bool isChoice = false )
		{
			bool returned = false;
			bool choice = false;

			TextPopup popup = new TextPopup
			{
				Text = message,
				IsChoice = isChoice
			};

			popup.Disappearing += ( s, e ) =>
			{
				if ( s is not TextPopup control )
					return;
				returned = true;
				choice = control.Choice;
			};

			await PopupNavigation.Instance.PushAsync( popup );

			while ( !returned )
				await Task.Delay( 50 );

			return choice;
		}

		public static async Task DisplayToast( string message, int time = 1000 )
		{
			if ( Application.Current.MainPage is NavigationPage mainPage )
			{
				await mainPage.DisplayToastAsync(
					new ToastOptions()
					{
						BackgroundColor = (Color)Application.Current.Resources[ColorStrings.SecondryBackground],
						MessageOptions = new MessageOptions
						{
							Message = message,
							Foreground = (Color)Application.Current.Resources[ColorStrings.Main]
						},
						Duration = TimeSpan.FromMilliseconds( time ),
						IsRtl = mainPage.IsRtl()
					} );
			}
		}
	}
}