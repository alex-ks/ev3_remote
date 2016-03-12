using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using SharpDX.DirectInput;

namespace Ev3Remote.Desktop.ViewModels
{
	public class MainViewModel : PropertyChangedBase, IDisposable
	{
		private readonly IWindowManager manager_;
		private readonly DirectInput _directInput;

		public MainViewModel( IWindowManager manager )
		{
			manager_ = manager;
			_directInput = new DirectInput( );
			Joysticks = new BindableCollection<JoystickViewModel>( );
			Scan( );
		}

		public BindableCollection<JoystickViewModel> Joysticks { get; set; }

		private void FillJoysticksFrom( IList<DeviceInstance> deviceList )
		{
			foreach ( var deviceInstance in deviceList )
			{
				if ( Joysticks.FirstOrDefault( j => j.Guid == deviceInstance.InstanceGuid ) == null )
				{
					Joysticks.Add( new JoystickViewModel( manager_ )
					{
						Name = deviceInstance.InstanceName.Trim( '\0' ),
						Connected = false,
						Guid = deviceInstance.InstanceGuid,
						Joystick = new Joystick( _directInput, deviceInstance.InstanceGuid )
					} );
				}
			}
		}

		public void Scan( )
		{
			var toDelete = new List<JoystickViewModel>( Joysticks.Where( j => !j.Connected ) );
			Joysticks.RemoveRange( toDelete );

			FillJoysticksFrom( _directInput.GetDevices( DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices ) );
			FillJoysticksFrom( _directInput.GetDevices( DeviceType.Joystick, DeviceEnumerationFlags.AllDevices ) );
		}

		public void Dispose( )
		{
			foreach ( var joystick in Joysticks )
			{
				joystick.Joystick.Dispose( );
			}
		}
	}
}
