using System;
using System.Device.Pwm;

namespace Common.Drivers
{
	/// <summary>
	/// Represents a rotary actuator or linear actuator that allows for precise control of angular or linear position.
	/// </summary>
	public sealed class ServoMotor : IDisposable
	{
		private PwmChannel _pwmChannel;
		private Double _maximumAngle;
		private Double _minimumPulseWidthMicroseconds;
		private Double _angleToMicroseconds;


		/// <summary>
		///  Initializes a new instance of the <see cref="ServoMotor"/> class.
		/// </summary>
		/// <param name="pwmChannel">The PWM channel used to control the servo motor.</param>
		/// <param name="maximumAngle">The maximum angle the servo motor can move represented as a value between 0 and 360.</param>
		/// <param name="minimumPulseWidthMicroseconds">The minimum pulse width, in microseconds, that represent an angle for 0 degrees.</param>
		/// <param name="maximumPulseWidthMicroseconds">The maxnimum pulse width, in microseconds, that represent an angle for maximum angle.</param>
		public ServoMotor(PwmChannel pwmChannel, Double maximumAngle = 180, Double minimumPulseWidthMicroseconds = 1_000, Double maximumPulseWidthMicroseconds = 2_000)
		{
			_pwmChannel = pwmChannel;
			Calibrate(maximumAngle, minimumPulseWidthMicroseconds, maximumPulseWidthMicroseconds);
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Sets calibration parameters
		/// </summary>
		/// <param name="maximumAngle">The maximum angle the servo motor can move represented as a value between 0 and 360.</param>
		/// <param name="minimumPulseWidthMicroseconds">The minimum pulse width, in microseconds, that represent an angle for 0 degrees.</param>
		/// <param name="maximumPulseWidthMicroseconds">The maxnimum pulse width, in microseconds, that represent an angle for maximum angle.</param>
		public void Calibrate(Double maximumAngle, Double minimumPulseWidthMicroseconds, Double maximumPulseWidthMicroseconds)
		{
			if(maximumAngle < 0 || maximumAngle > 360)
			{
				throw new ArgumentOutOfRangeException(nameof(maximumAngle), maximumAngle, "Value must be between 0 and 360.");
			}

			if(minimumPulseWidthMicroseconds < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minimumPulseWidthMicroseconds), minimumPulseWidthMicroseconds, "Value must not be negative.");
			}

			if(maximumPulseWidthMicroseconds < minimumPulseWidthMicroseconds)
			{
				throw new ArgumentOutOfRangeException(nameof(maximumPulseWidthMicroseconds), maximumPulseWidthMicroseconds, $"Value must be greater than or equal to {minimumPulseWidthMicroseconds}.");
			}

			_maximumAngle = maximumAngle;
			_minimumPulseWidthMicroseconds = minimumPulseWidthMicroseconds;
			_angleToMicroseconds = (maximumPulseWidthMicroseconds - minimumPulseWidthMicroseconds) / maximumAngle;
		}

		/// <summary>
		/// Starts the servo motor.
		/// </summary>
		public void Start() => _pwmChannel.Start();

		/// <summary>
		/// Stops the servo motor.
		/// </summary>
		public void Stop() => _pwmChannel.Stop();

		/// <summary>
		/// Writes an angle to the servo motor.
		/// </summary>
		/// <param name="angle">The angle to write to the servo motor.</param>
		public void WriteAngle(Double angle)
		{
			if(angle < 0 || angle > _maximumAngle)
			{
				throw new ArgumentOutOfRangeException(nameof(angle), angle, $"Value must be between 0 and {_maximumAngle}.");
			}

			WritePulseWidth((Int32)(_angleToMicroseconds * angle + _minimumPulseWidthMicroseconds));
		}

		/// <summary>
		/// Writes a pulse width to the servo motor.
		/// </summary>
		/// <param name="microseconds">The pulse width, in microseconds, to write to the servo motor.</param>
		public void WritePulseWidth(Int32 microseconds)
		{
			Double dutyCycle = (Double)microseconds / 1_000_000 * _pwmChannel.Frequency; // Convert to seconds 1st.
			_pwmChannel.DutyCycle = dutyCycle;
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		public void Dispose()
		{
			_pwmChannel?.Dispose();
			_pwmChannel = null;
		}
	}
}
