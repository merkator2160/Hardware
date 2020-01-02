using System;
using System.Device.Gpio;
using System.Device.Pwm;

namespace Common.Drivers.DcMotor
{
	/// <summary>
	/// Direct current (DC) motor
	/// </summary>
	public abstract class DcMotorBase : IDisposable
	{
		private const Int32 DefaultPwmFrequency = 50;



		/// <summary>
		/// Constructs generic <see cref="DcMotorBase"/> instance
		/// </summary>
		/// <param name="controller"><see cref="GpioController"/> related with operations on pins</param>
		protected DcMotorBase(GpioController controller)
		{
			Controller = controller;
		}

		/// <summary>
		/// <see cref="GpioController"/> related with operations on pins
		/// </summary>
		protected GpioController Controller
		{
			get;
			set;
		}



		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets or sets the speed of the motor. Range is -1..1 or 0..1 for 1-pin connection.
		/// 1 means maximum speed, 0 means no movement and -1 means movement in opposite direction.
		/// </summary>
		public abstract Double Speed { get; set; }


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Creates <see cref="DcMotorBase"/> instance which allows to control speed in one direction.
		/// </summary>
		/// <param name="speedControlChannel"><see cref="PwmChannel"/> used to control the speed of the motor</param>
		/// <returns>DCMotor instance</returns>
		/// <remarks>
		/// PWM pin <paramref name="speedControlChannel"/> can be connected to either enable pin of the H-bridge.
		/// or directly to the input related with the motor (if H-bridge allows inputs to change frequently).
		/// Connecting motor directly to GPIO pin is not recommended and may damage your board.
		/// </remarks>
		public static DcMotorBase Create(PwmChannel speedControlChannel)
		{
			if(speedControlChannel == null)
			{
				throw new ArgumentNullException(nameof(speedControlChannel));
			}

			return new DcMotor2PinNoEnableDriver(speedControlChannel, -1, null);
		}

		/// <summary>
		/// Creates <see cref="DcMotorBase"/> instance which allows to control speed in one direction.
		/// </summary>
		/// <param name="speedControlPin">Pin used to control the speed of the motor with software PWM (frequency will default to 50Hz)</param>
		/// <param name="controller"><see cref="GpioController"/> related to the <paramref name="speedControlPin"/></param>
		/// <returns><see cref="DcMotorBase"/> instance</returns>
		/// <remarks>
		/// <paramref name="speedControlPin"/> can be connected to either enable pin of the H-bridge.
		/// or directly to the input related with the motor (if H-bridge allows inputs to change frequently).
		/// Connecting motor directly to GPIO pin is not recommended and may damage your board.
		/// </remarks>
		public static DcMotorBase Create(Int32 speedControlPin, GpioController controller = null)
		{
			if(speedControlPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(speedControlPin));
			}

			controller = controller ?? new GpioController();
			return new DcMotor2PinNoEnableDriver(new SoftwarePwmChannel(speedControlPin, DefaultPwmFrequency, 0.0, controller: controller), -1, controller);
		}

		/// <summary>
		/// Creates <see cref="DcMotorBase"/> instance which allows to control speed in both directions.
		/// </summary>
		/// <param name="speedControlChannel"><see cref="PwmChannel"/> used to control the speed of the motor</param>
		/// <param name="directionPin">Pin used to control the direction of the motor</param>
		/// <param name="controller"><see cref="GpioController"/> related to the <paramref name="directionPin"/></param>
		/// <returns><see cref="DcMotorBase"/> instance</returns>
		/// <remarks>
		/// <paramref name="speedControlChannel"/> can be connected to either enable pin of the H-bridge.
		/// or directly to the input related with the motor (if H-bridge allows inputs to change frequently).
		/// <paramref name="directionPin"/> should be connected to H-bridge input corresponding to one of the motor inputs.
		/// Connecting motor directly to GPIO pin is not recommended and may damage your board.
		/// </remarks>
		public static DcMotorBase Create(PwmChannel speedControlChannel, Int32 directionPin, GpioController controller = null)
		{
			if(speedControlChannel == null)
			{
				throw new ArgumentNullException(nameof(speedControlChannel));
			}

			if(directionPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(directionPin));
			}

			return new DcMotor2PinNoEnableDriver(speedControlChannel, directionPin, controller);
		}

		/// <summary>
		/// Creates <see cref="DcMotorBase"/> instance which allows to control speed in both directions.
		/// </summary>
		/// <param name="speedControlPin">Pin used to control the speed of the motor with software PWM (frequency will default to 50Hz)</param>
		/// <param name="directionPin">Pin used to control the direction of the motor</param>
		/// <param name="controller">GPIO controller related to <paramref name="speedControlPin"/> and <paramref name="directionPin"/></param>
		/// <returns><see cref="DcMotorBase"/> instance</returns>
		/// <remarks>
		/// PWM pin <paramref name="speedControlPin"/> can be connected to either enable pin of the H-bridge.
		/// or directly to the input related with the motor (if H-bridge allows inputs to change frequently).
		/// <paramref name="directionPin"/> should be connected to H-bridge input corresponding to one of the motor inputs.
		/// Connecting motor directly to GPIO pin is not recommended and may damage your board.
		/// </remarks>
		public static DcMotorBase Create(Int32 speedControlPin, Int32 directionPin, GpioController controller = null)
		{
			if(speedControlPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(speedControlPin));
			}

			if(directionPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(directionPin));
			}

			controller = controller ?? new GpioController();
			return new DcMotor2PinNoEnableDriver(new SoftwarePwmChannel(speedControlPin, DefaultPwmFrequency, 0.0, controller: controller), directionPin, controller);
		}

		/// <summary>
		/// Creates <see cref="DcMotorBase"/> instance which allows to control speed in both directions.
		/// </summary>
		/// <param name="speedControlChannel"><see cref="PwmChannel"/> used to control the speed of the motor</param>
		/// <param name="directionPin">First pin used to control the direction of the motor</param>
		/// <param name="otherDirectionPin">Second pin used to control the direction of the motor</param>
		/// <param name="controller"><see cref="GpioController"/> related to <paramref name="directionPin"/> and <paramref name="otherDirectionPin"/></param>
		/// <returns><see cref="DcMotorBase"/> instance</returns>
		/// <remarks>
		/// When speed is non-zero the value of <paramref name="otherDirectionPin"/> will always be opposite to that of <paramref name="directionPin"/>.
		/// <paramref name="speedControlChannel"/> should be connected to enable pin of the H-bridge.
		/// <paramref name="directionPin"/> should be connected to H-bridge input corresponding to one of the motor inputs.
		/// <paramref name="otherDirectionPin"/> should be connected to H-bridge input corresponding to the remaining motor input.
		/// Connecting motor directly to GPIO pin is not recommended and may damage your board.
		/// </remarks>
		public static DcMotorBase Create(PwmChannel speedControlChannel, Int32 directionPin, Int32 otherDirectionPin, GpioController controller = null)
		{
			if(speedControlChannel == null)
			{
				throw new ArgumentNullException(nameof(speedControlChannel));
			}

			if(directionPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(directionPin));
			}

			if(otherDirectionPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(otherDirectionPin));
			}

			return new DcMotor3PinDriver(
				speedControlChannel,
				directionPin,
				otherDirectionPin,
				controller);
		}

		/// <summary>
		/// Creates <see cref="DcMotorBase"/> instance which allows to control speed in both directions.
		/// </summary>
		/// <param name="speedControlPin">Pin used to control the speed of the motor with software PWM (frequency will default to 50Hz)</param>
		/// <param name="directionPin">First pin used to control the direction of the motor</param>
		/// <param name="otherDirectionPin">Second pin used to control the direction of the motor</param>
		/// <param name="controller"><see cref="GpioController"/> related to <paramref name="speedControlPin"/>, <paramref name="directionPin"/> and <paramref name="otherDirectionPin"/></param>
		/// <returns><see cref="DcMotorBase"/> instance</returns>
		/// <remarks>
		/// When speed is non-zero the value of <paramref name="otherDirectionPin"/> will always be opposite to that of <paramref name="directionPin"/>
		/// PWM pin <paramref name="speedControlPin"/> should be connected to enable pin of the H-bridge.
		/// <paramref name="directionPin"/> should be connected to H-bridge input corresponding to one of the motor inputs.
		/// <paramref name="otherDirectionPin"/> should be connected to H-bridge input corresponding to the remaining motor input.
		/// Connecting motor directly to GPIO pin is not recommended and may damage your board.
		/// </remarks>
		public static DcMotorBase Create(Int32 speedControlPin, Int32 directionPin, Int32 otherDirectionPin, GpioController controller = null)
		{
			if(speedControlPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(speedControlPin));
			}

			if(directionPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(directionPin));
			}

			if(otherDirectionPin == -1)
			{
				throw new ArgumentOutOfRangeException(nameof(otherDirectionPin));
			}

			controller = controller ?? new GpioController();
			return new DcMotor3PinDriver(new SoftwarePwmChannel(speedControlPin, DefaultPwmFrequency, 0.0, controller: controller), directionPin, otherDirectionPin, controller);
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Disposes the <see cref="DcMotorBase"/> class
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources used by the <see cref="DcMotorBase"/> instance.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(Boolean disposing)
		{
			if(disposing)
			{
				Controller?.Dispose();
				Controller = null;
			}
		}
	}
}
