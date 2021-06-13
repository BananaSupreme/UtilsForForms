using Celery.ViewModels;

using System;
using System.Threading.Tasks;

namespace Celery.Services
{
	public interface INavigationService
	{
		ViewModel PreviousPageViewModel { get; }

		void InitializeAsync();
		Task ReturnToMain();
		Task NavigateToAsync<TViewModel>() where TViewModel : ViewModel;

		Task NavigateToAsync<TViewModel>( object parameter ) where TViewModel : ViewModel;

		Task PopAsync();

		Task RemoveLastFromBackStackAsync();

		Task RemoveBackStackAsync();

		void RegisterForNavigation( Type viewModel, Type page );
	}
}
