using Celery.Helpers;
using Celery.ViewModels;
using Celery.Views;
using Celery.Views.Popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Celery.Services
{
	public class NavigationService : INavigationService
	{
		private static readonly LoadingPopup _loading = new LoadingPopup();
		private static int _loadingCount;

		public static INavigationService Instance = new NavigationService();

		public static async Task RaiseLoading()
		{
			try
			{
				if ( _loadingCount == 0 )
					await PopupNavigation.Instance.PushAsync( _loading );
				_loadingCount++;
			}
			catch ( Rg.Plugins.Popup.Exceptions.RGPageInvalidException ) { }
		}

		public static async Task RemoveLoading()
		{
			if ( _loadingCount <= 1 )
				await PopupNavigation.Instance.RemovePageAsync( _loading );
			if ( _loadingCount > 0 )
				_loadingCount--;
		}

		public ViewModel PreviousPageViewModel
		{
			get
			{
				var mainPage = Application.Current.MainPage as CustomNavigationPage;
				var viewModel = mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2].BindingContext;
				return viewModel as ViewModel;
			}
		}

		private readonly Dictionary<Type, Type> _registeredPages;

		public NavigationService()
		{
			_registeredPages = new Dictionary<Type, Type>();

			RegisterForNavigation( typeof( MainViewModel ), typeof( MainView ) );
			RegisterForNavigation( typeof( ReportViewModel ), typeof( ReportView ) );
			RegisterForNavigation( typeof( JobDetailsViewModel ), typeof( JobDetailsView ) );
			RegisterForNavigation( typeof( JobSelectionViewModel ), typeof( JobSelectionView ) );
			RegisterForNavigation( typeof( SplashViewModel ), typeof( SplashView ) );
			RegisterForNavigation( typeof( TestViewModel ), typeof( TestView ) );
			RegisterForNavigation( typeof( PayEstimationViewModel ), typeof( PayEstimationView ) );
			RegisterForNavigation( typeof( TransitionViewModel ), typeof( TransitionView ) );
		}

		public void InitializeAsync()
		{
			Application.Current.MainPage = new SplashView();
		}

		public async Task ReturnToMain()
		{
			if ( Application.Current.MainPage.GetType() == typeof( MainView ) && Application.Current.MainPage.Navigation.NavigationStack.Count == 2 )
			{
				await PopAsync();
			}
			else
			{
				await NavigateToAsync<MainViewModel>();
				await RemoveBackStackAsync();
			}
			Global.RefreshMain();
		}

		public async Task NavigateToAsync<TViewModel>() where TViewModel : ViewModel
		{
			await RaiseLoading();
			await InternalNavigateToAsync( typeof( TViewModel ), null );
			await RemoveLoading();
		}

		public async Task NavigateToAsync<TViewModel>( object parameter ) where TViewModel : ViewModel
		{
			await RaiseLoading();
			await InternalNavigateToAsync( typeof( TViewModel ), parameter );
			await RemoveLoading();
		}

		public Task PopAsync()
		{
			if ( Application.Current.MainPage is CustomNavigationPage mainPage )
			{
				mainPage.Navigation.PopAsync();
			}

			return Task.FromResult( true );
		}

		public Task RemoveLastFromBackStackAsync()
		{
			if ( Application.Current.MainPage is CustomNavigationPage mainPage )
			{
				if ( mainPage.Navigation.NavigationStack.Count - 2 > -1 )
					mainPage.Navigation.RemovePage(
						mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2] );
			}

			return Task.FromResult( true );
		}

		public Task RemoveBackStackAsync()
		{
			if ( Application.Current.MainPage is CustomNavigationPage mainPage )
			{
				for ( int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++ )
				{
					var page = mainPage.Navigation.NavigationStack[i];
					mainPage.Navigation.RemovePage( page );
				}
			}

			return Task.FromResult( true );
		}

		public void RegisterForNavigation( Type viewModelType, Type pageType )
		{
			_registeredPages.Add( viewModelType, pageType );
		}

		private async Task InternalNavigateToAsync( Type viewModelType, object parameter )
		{
			Page page = CreatePage( viewModelType, parameter );

			if ( Application.Current.MainPage is CustomNavigationPage navigationPage )
			{
				await navigationPage.PushAsync( page );
			}
			else
			{
				Device.BeginInvokeOnMainThread( () => Application.Current.MainPage = new CustomNavigationPage( page ) );
			}
			await ( page.BindingContext as ViewModel ).InitializeAsync( parameter );
		}

		private Type GetPageTypeForViewModel( Type viewModelType )
		{
			if ( !_registeredPages.ContainsKey( viewModelType ) )
				throw new Exception( "Page Not Found" );

			return _registeredPages[viewModelType];
		}

		private Page CreatePage( Type viewModelType, object parameter )
		{
			Type pageType = GetPageTypeForViewModel( viewModelType );

			MethodInfo method = typeof( Resolver ).GetMethod( nameof( Resolver.Resolve ) );
			MethodInfo generic = method.MakeGenericMethod( pageType );

			Page page = generic.Invoke( null, null ) as Page;
			return page;
		}
	}
}