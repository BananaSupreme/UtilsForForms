using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Celery.Controls
{
	[XamlCompilation( XamlCompilationOptions.Compile )]
	public partial class CheckboxWithTextControl : ContentView
	{
		public static readonly BindableProperty TextProperty =
			BindableProperty.Create( "Text", typeof( string ), typeof( CheckboxWithTextControl ), "Default", BindingMode.OneWay,
				propertyChanged: ( bindable, oldValue, newValue ) =>
				{
					if ( newValue != null && bindable is CheckboxWithTextControl control )
					{
						control.DisplayedLabel.Text = (string)newValue;
					}
				} );

		public static readonly BindableProperty IsCheckedProperty =
			BindableProperty.Create( "IsChecked", typeof( bool ), typeof( CheckboxWithTextControl ), false, BindingMode.TwoWay,
				propertyChanged: ( BindableObject bindable, object oldValue, object newValue ) =>
				{
					if ( bindable is CheckboxWithTextControl control && newValue is bool value )
					{
						control.DisplayedCheckbox.IsChecked = value;
						control.CheckedChanged?.Invoke( control, new CheckedChangedEventArgs( value ) );
					}
				} );

		public string Text
		{
			get { return (string)GetValue( TextProperty ); }
			set { SetValue( TextProperty, value ); }
		}

		public bool IsChecked
		{
			get { return (bool)GetValue( IsCheckedProperty ); }
			set { SetValue( IsCheckedProperty, value ); }
		}

		public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

		public CheckboxWithTextControl()
		{
			InitializeComponent();
		}

		private void OnTapped( object sender, EventArgs e )
		{
			IsChecked = !IsChecked;
		}

		private void OnCheckedChange( object sender, CheckedChangedEventArgs e )
		{
			if ( IsChecked != e.Value )
				IsChecked = e.Value;
		}
	}
}