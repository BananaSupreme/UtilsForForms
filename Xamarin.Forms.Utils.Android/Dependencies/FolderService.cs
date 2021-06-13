using Android.Content;
using Android.App;
using Xamarin.Forms;
using Xamarin.Forms.Utils.Droid.Dependencies;
using Xamarin.Forms.Utils.Services;

[assembly: Dependency( typeof( FolderService ) )]

namespace Xamarin.Forms.Utils.Droid.Dependencies
{
	public class FolderService : IFolderService
	{
		private readonly Context _context = Android.App.Application.Context;

		public string GetDocumentsFolder()
		{
			return _context.GetExternalFilesDir( Android.OS.Environment.DirectoryDocuments ).AbsolutePath;
		}
	}
}