using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using SharpDX.DirectInput;
using DeviceType = SharpDX.DirectInput.DeviceType;


namespace Ev3Remote
{
	public class Launcher
	{
		public static void MainBrick( string[] args )
		{
			Brick brick = new Brick( new BluetoothCommunication( "COM3" ) );
			var task = brick.ConnectAsync( TimeSpan.FromSeconds( 2 ) );
			task.Wait( );
			brick.DirectCommand.TurnMotorAtPowerAsync( OutputPort.B, 75 );
			brick.DirectCommand.StopMotorAsync( OutputPort.B, false );
		}

		public static void Main( string[] args )
		{
			using ( var directInput = new DirectInput( ) )
			{
				var gamepadGuid = Guid.Empty;

				foreach ( var deviceInstance in directInput.GetDevices( DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices ) )
				{
					gamepadGuid = deviceInstance.InstanceGuid;
					Console.WriteLine( deviceInstance.InstanceName );
					break;
				}

				if ( gamepadGuid == Guid.Empty )
				{
					foreach ( var deviceInstance in directInput.GetDevices( DeviceType.Joystick, DeviceEnumerationFlags.AllDevices ) )
					{
						gamepadGuid = deviceInstance.InstanceGuid;
						Console.WriteLine( deviceInstance.InstanceName );
						break;
					}
				}

				if ( gamepadGuid == Guid.Empty )
				{
					Console.Out.WriteLine( "No gamepads" );
					Console.ReadKey( );
					return;
				}


				using ( var gamepad = new Joystick( directInput, gamepadGuid ) )
				{
					gamepad.Properties.BufferSize = 128;
					gamepad.Acquire( );

					Console.WriteLine( gamepad.ToString( ) );

					while ( true )
					{
						gamepad.Poll( );
						var data = gamepad.GetBufferedData( );

						var topPos = Console.CursorTop;

						foreach ( var state in data )
						{
							Console.WriteLine( state );
						}
						Console.SetCursorPosition( 0, topPos );
					}
				}
			}
		}
	}
}
