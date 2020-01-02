using System;
using System.Device.Gpio;
using System.Device.Pwm;

namespace Common.Drivers.DcMotor
{
	internal class DcMotor3PinDriver : DcMotorBase
	{
		private PwmChannel _pwm;
		private Int32 _pin0;
		private Int32 _pin1;
		private Double _speed;


		public DcMotor3PinDriver(PwmChannel pwmChannel, Int32 pin0, Int32 pin1, GpioController controller) : base(controller ?? new GpioController())
		{
			if(pwmChannel == null)
				throw new ArgumentNullException(nameof(pwmChannel));

			_pwm = pwmChannel;

			_pin0 = pin0;
			_pin1 = pin1;

			_speed = 0;

			_pwm.Start();

			Controller.OpenPin(_pin0, PinMode.Output);
			Controller.Write(_pin0, PinValue.Low);

			Controller.OpenPin(_pin1, PinMode.Output);
			Controller.Write(_pin1, PinValue.Low);
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets or sets the speed of the motor.
		/// Speed is a value from -1 to 1.
		/// 1 means maximum speed, signed value changes the direction.
		/// </summary>
		public override Double Speed
		{
			get
			{
				return _speed;
			}
			set
			{
				var val = Math.Clamp(value, -1.0, 1.0);

				if(_speed == val)
				{
					return;
				}

				if(val == 0.0)
				{
					Controller.Write(_pin0, PinValue.Low);
					Controller.Write(_pin1, PinValue.Low);
				}
				else if(val > 0.0)
				{
					Controller.Write(_pin0, PinValue.Low);
					Controller.Write(_pin1, PinValue.High);
				}
				else
				{
					Controller.Write(_pin0, PinValue.High);
					Controller.Write(_pin1, PinValue.Low);
				}

				_pwm.DutyCycle = Math.Abs(val);

				_speed = val;
			}
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected override void Dispose(Boolean disposing)
		{
			if(disposing)
			{
				_speed = 0.0;
				_pwm?.Dispose();
				_pwm = null;
			}

			base.Dispose(disposing);
		}
	}
}
