using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Utils.Controls.Rotator
{
	public class RotatorShiftedEventArgs : EventArgs
	{
		public RotatorShift rotatorShift;

		public RotatorShiftedEventArgs( RotatorShift rotatorShift )
		{
			this.rotatorShift = rotatorShift;
		}
	}
}