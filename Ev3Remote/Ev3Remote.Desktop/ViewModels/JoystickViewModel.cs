using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using SharpDX.DirectInput;

namespace Ev3Remote.Desktop.ViewModels
{
	public class JoystickViewModel : PropertyChangedBase
	{
		private IWindowManager _manager;

		internal Guid Guid { get; set; }

		public string Name { get; set; }

		private bool _connected;

		public JoystickViewModel( IWindowManager manager )
		{
			_manager = manager;
		}

		public bool Connected
		{
			get { return _connected; }

			set
			{
				if ( value != _connected )
				{
					_connected = value;
					NotifyOfPropertyChange( ( ) => Connected );
					NotifyOfPropertyChange( ( ) => NotConnected );
				}
			}
		}

		public bool NotConnected => !Connected;

		public Joystick Joystick { get; set; }

		public void Connect( )
		{
			_manager.ShowWindow( new ControlViewModel( this ) );
		}
	}
}
