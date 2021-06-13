using Celery.Resources;

using System;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace Celery.Services
{
	public class PermissionService : IPermissionService
	{
		public async Task<bool> ValidateReadWritePermission()
		{
			PermissionStatus permission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
			if ( permission != PermissionStatus.Granted )
			{
				await DialogService.RaiseMessageAsync( Text.ReadWritePermissionRequest );
				permission = await Permissions.RequestAsync<Permissions.StorageWrite>();
			}

			return permission == PermissionStatus.Granted;
		}
	}
}