using System.Globalization;

using Java.Util;

namespace Xamarin.Forms.Utils.Droid.Helpers
{
	public class LanguageWorkAround1866
	{
		public static void FixLanguageError()
		{
			// Mono mishandles Hebrew, Indonesian, and Arabic identifiers and sets the app to the invariant culture.
			Locale locale = Locale.GetDefault( Locale.Category.Display );
			CultureInfo culture = locale.ISO3Language switch
			{
				"heb" => CultureInfo.GetCultureInfo( "he" ),
				"ind" => CultureInfo.GetCultureInfo( "id" ),
				"ara" => CultureInfo.GetCultureInfo( "ar" ),
				_ => CultureInfo.CurrentUICulture
			};

			CultureInfo.CurrentCulture = culture;
			CultureInfo.CurrentUICulture = culture;
		}
	}
}