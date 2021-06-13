namespace Xamarin.Forms.Utils
{
	public static class IntExtensions
	{
		public static int Mod( this int x, int m )
		{
			return m == 0 ? 0 : ( x % m + m ) % m;
		}
	}
}