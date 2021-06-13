using System.Threading.Tasks;

using Xamarin.CommunityToolkit.ObjectModel;

namespace Xamarin.Forms.Utils.ViewModels
{
	public abstract class BaseViewModel : ObservableObject
	{
		private bool _isBusy;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty( ref _isBusy, value );
		}

		public BaseViewModel()
		{
		}

		public virtual Task InitializeAsync( object navigationData )
		{
			return Task.FromResult( true );
		}
	}
}