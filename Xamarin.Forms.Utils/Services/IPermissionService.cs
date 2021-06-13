using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Celery.Services
{
	public interface IPermissionService
	{
		public Task<bool> ValidateReadWritePermission();
	}
}