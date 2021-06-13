using System.Windows.Input;

namespace Xamarin.Forms.Utils
{
	public static class ICommandExtensions
	{
		public static void SafeExecute( this ICommand command, object parameter = null )
		{
			if ( command == null )
				return;
			if ( command.CanExecute( parameter ) )
			{
				command.Execute( parameter );
			}
		}
	}
}