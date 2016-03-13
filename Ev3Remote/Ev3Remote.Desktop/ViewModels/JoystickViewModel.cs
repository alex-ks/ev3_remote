using System;
using System.Collections.Concurrent;
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
		private readonly IWindowManager _manager;

		private static readonly Dictionary<Guid, string> UsedComPorts = new Dictionary<Guid, string>( );
		private static readonly object LockGuard = new object( );

		internal Guid Guid { get; set; }

		public string Name { get; set; }

		private string _comPortName;

		public string ComPortName
		{
			get { return _comPortName; }
			set
			{
				if ( _comPortName != value )
				{
					lock ( LockGuard )
					{
						if ( !UsedComPorts.ContainsKey( Guid ) )
						{
							UsedComPorts.Add( Guid, value );
						}
						else
						{
							UsedComPorts[Guid] = value;
						}
						_comPortName = value;
					}
				}
			}
		}

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
			lock ( LockGuard )
			{
				if ( !UsedComPorts.FirstOrDefault( pair => pair.Key != Guid && pair.Value == ComPortName ).Equals( default( KeyValuePair<Guid, string> ) ) )
				{
					MessageBox.Show( Resources.ComPortUsed, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Warning );
					return;
				}
			}

			_manager.ShowWindow( new ControlViewModel( this ) );
		}
	}
}
