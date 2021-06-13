using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xamarin.Forms.Utils
{
	public static class EnumExtensions
	{
		public static string GetDescription<TEnum>( this TEnum _, object value ) where TEnum : Enum
		{
			string result = value.ToString();

			DisplayAttribute attribute = typeof( TEnum ).GetRuntimeField( value.ToString() ).GetCustomAttributes<DisplayAttribute>( false ).SingleOrDefault();
			if ( attribute != null )
				result = attribute.Description;
			//else
			//{
			//	//is there a resource entry?
			//	var match = Text.ResourceManager.GetString( $"{typeof( TEnum ).Name}_{value.ToString().ToString( CultureInfo.InvariantCulture )}" );
			//	if ( !string.IsNullOrWhiteSpace( match ) )
			//		result = match;
			//}

			return result;
		}

		public static TEnum GetValueByDescription<TEnum>( this TEnum _, string description ) where TEnum : Enum
		{
			return Enum.GetValues( typeof( TEnum ) ).Cast<TEnum>().FirstOrDefault( x => string.Equals( _.GetDescription( x ), description ) );
		}

		public static IEnumerable<string> GetAllDescriptions<TEnum>( this TEnum _ ) where TEnum : Enum
		{
			foreach ( object value in Enum.GetValues( typeof( TEnum ) ) )
			{
				yield return _.GetDescription( value );
			}
		}
	}
}