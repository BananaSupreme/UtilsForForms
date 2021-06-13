using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms.Utils.Controls.WheelPickerControl
{
	public class WheelValue : INotifyPropertyChanged
	{
		public WheelValue( string text, double size )
		{
			Text = text;
			Size = size;
		}

		private string _text;

		public string Text
		{
			get => _text;
			set
			{
				if ( _text != value )
				{
					_text = value;
					PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( Text ) ) );
				}
			}
		}

		private double _size;

		public double Size
		{
			get => _size;
			set
			{
				if ( _size != value )
				{
					_size = value;
					PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( Size ) ) );
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public override bool Equals( object obj )
		{
			return obj is WheelValue value &&
				   Text == value.Text;
		}

		public override int GetHashCode()
		{
			return 1249999374 + EqualityComparer<string>.Default.GetHashCode( Text );
		}

		public static bool operator ==( WheelValue left, WheelValue right )
		{
			return EqualityComparer<WheelValue>.Default.Equals( left, right );
		}

		public static bool operator !=( WheelValue left, WheelValue right )
		{
			return !( left == right );
		}
	}
}