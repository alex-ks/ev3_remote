﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using SharpDX.DirectInput;
using Action = System.Action;
using Color = System.Windows.Media.Color;

namespace Ev3Remote.Desktop.ViewModels
{
	public class ControlViewModel : Screen
	{
		private const int PollPeriod = 20;
		private const int WaitPeriod = 2000;

		private static readonly SolidColorBrush GhostWhite = new SolidColorBrush( new Color { A = 0 } );
		private static readonly SolidColorBrush OrangeRed = new SolidColorBrush( Colors.OrangeRed );

		private readonly JoystickViewModel _joyModel;
		private SolidColorBrush _pos0, _pos1, _pos2, _pos3, _pos4, _pos5, _pos6, _pos7, _action;

		private readonly Brick _brick;
		private bool _connected;

		public string Name { get; set; }

		public OutputPort LeftMotor { get; set; } = OutputPort.B;
		public OutputPort RightMotor { get; set; } = OutputPort.D;

		public ControlViewModel( JoystickViewModel joyModel )
		{
			Name = joyModel.Name;
			_joyModel = joyModel;
			ClearPositions( );
			ActionButton = GhostWhite;
			_brick = new Brick( new BluetoothCommunication( _joyModel.ComPortName ) );
			Action listening = Listen;
			listening.BeginInvoke( OnListeningEnd, null );
		}

		private void HandleDirectionChange( int value )
		{
			int leftPower = 0, rightPower = 0;
			switch ( value )
			{
				case 0:
					Position0 = OrangeRed;
					//Position0 = "N";
					leftPower = rightPower = 80;
					break;
				case 4500:
					Position1 = OrangeRed;
					//Position1 = "NE";
					leftPower = 80;
					rightPower = 40;
					break;
				case 9000:
					Position2 = OrangeRed;
					//Position2 = "E";
					leftPower = 80;
					rightPower = 0;
					break;
				case 13500:
					Position3 = OrangeRed;
					//Position3 = "SE";
					leftPower = -80;
					rightPower = -40;
					break;
				case 18000:
					Position4 = OrangeRed;
					//Position4 = "S";
					leftPower = rightPower = -80;
					break;
				case 22500:
					Position5 = OrangeRed;
					//Position5 = "SW";
					leftPower = -40;
					rightPower = -80;
					break;
				case 27000:
					Position6 = OrangeRed;
					//Position6 = "W";
					leftPower = 0;
					rightPower = 80;
					break;
				case 31500:
					Position7 = OrangeRed;
					//Position7 = "NW";
					leftPower = 40;
					rightPower = 80;
					break;
				case -1:
					ClearPositions( );
					leftPower = rightPower = 0;
					break;
			}

			_brick.DirectCommand.TurnMotorAtPowerAsync( LeftMotor, leftPower );
			_brick.DirectCommand.TurnMotorAtPowerAsync( RightMotor, rightPower );
		}

		private void HandleActionButtonChange( int value )
		{
			ActionButton = value != 0 ? OrangeRed : GhostWhite;
		}

		private void Listen( )
		{
			var joystick = _joyModel.Joystick;
			_joyModel.Connected = true;
			_connected = true;

			try
			{
				var connectTask = _brick.ConnectAsync( TimeSpan.Zero );
				connectTask.Wait( WaitPeriod );

				if ( !connectTask.IsCompleted )
				{
					throw new TimeoutException( Resources.ConnectTimeout );
				}

				joystick.Properties.BufferSize = 128;
				joystick.Acquire( );

				while ( _connected )
				{
					joystick.Poll( );
					var data = joystick.GetBufferedData( );

					foreach ( var update in data )
					{
						ClearPositions( );
						switch ( update.Offset )
						{
							case JoystickOffset.Buttons0:
								HandleActionButtonChange( update.Value );
								break;
							case JoystickOffset.PointOfViewControllers0:
								HandleDirectionChange( update.Value );
								break;
						}
					}
					Thread.Sleep( PollPeriod );
				}
			}
			catch ( Exception e )
			{
				MessageBox.Show( e.Message );
				_joyModel.Connected = false;
			}
			finally
			{
				SelfDispose( );
			}
		}

		private void SelfDispose( )
		{
			try
			{
				_joyModel.Joystick.Unacquire( );
			}
			catch ( Exception )
			{
				// ignored
			}
			try
			{
				_brick.Disconnect( );
			}
			catch ( Exception )
			{
				// ignored
			}
		}

		private void ClearPositions( )
		{
			Position0 =
				Position1 =
					Position2 =
						Position3 =
							Position4 =
								Position5 =
									Position6 =
										Position7 = GhostWhite;
		}

		public void OnClose( )
		{
			_connected = false;
			SelfDispose( );
			new TaskFactory( ).StartNew( ( ) =>
			{
				Thread.Sleep( 3000 );
				_joyModel.Connected = false;
			} );
		}

		public void OnListeningEnd( IAsyncResult result )
		{
			TryClose( );
		}

		public SolidColorBrush Position0
		{
			get { return _pos0; }
			set
			{
				if ( _pos0 != value )
				{
					_pos0 = value;
					NotifyOfPropertyChange( ( ) => Position0 );
				}
			}
		}

		public SolidColorBrush Position1
		{
			get { return _pos1; }
			set
			{
				if ( _pos1 != value )
				{
					_pos1 = value;
					NotifyOfPropertyChange( ( ) => Position1 );
				}
			}
		}

		public SolidColorBrush Position2
		{
			get { return _pos2; }
			set
			{
				if ( _pos2 != value )
				{
					_pos2 = value;
					NotifyOfPropertyChange( ( ) => Position2 );
				}
			}
		}

		public SolidColorBrush Position3
		{
			get { return _pos3; }
			set
			{
				if ( _pos3 != value )
				{
					_pos3 = value;
					NotifyOfPropertyChange( ( ) => Position3 );
				}
			}
		}

		public SolidColorBrush Position4
		{
			get { return _pos4; }
			set
			{
				if ( _pos4 != value )
				{
					_pos4 = value;
					NotifyOfPropertyChange( ( ) => Position4 );
				}
			}
		}

		public SolidColorBrush Position5
		{
			get { return _pos5; }
			set
			{
				if ( _pos5 != value )
				{
					_pos5 = value;
					NotifyOfPropertyChange( ( ) => Position5 );
				}
			}
		}

		public SolidColorBrush Position6
		{
			get { return _pos6; }
			set
			{
				if ( _pos6 != value )
				{
					_pos6 = value;
					NotifyOfPropertyChange( ( ) => Position6 );
				}
			}
		}

		public SolidColorBrush Position7
		{
			get { return _pos7; }
			set
			{
				if ( _pos7 != value )
				{
					_pos7 = value;
					NotifyOfPropertyChange( ( ) => Position7 );
				}
			}
		}

		public SolidColorBrush ActionButton
		{
			get { return _action; }
			set
			{
				if ( _action != value )
				{
					_action = value;
					NotifyOfPropertyChange( ( ) => ActionButton );
				}
			}
		}
	}
}
