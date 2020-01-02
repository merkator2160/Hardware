using System;
using System.Device.Gpio;
using System.Device.Pwm;

namespace Common.Drivers.DcMotor
{
	internal class DcMotor2PinNoEnableDriver : DcMotorBase
	{
		private PwmChannel _pwm;
		private Int32 _pin1;
		private Double _speed;


		public DcMotor2PinNoEnableDriver(PwmChannel pwmChannel, Int32 pin1, GpioController controller) : base(controller ?? ((pin1 == -1) ? null : new GpioController()))
		{
			_pwm = pwmChannel;

			_pin1 = pin1;

			_speed = 0;

			_pwm.Start();

			if(_pin1 != -1)
			{
				Controller.OpenPin(_pin1, PinMode.Output);
				Controller.Write(_pin1, PinValue.Low);
			}
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets or sets the speed of the motor.
		/// Speed is a value from 0 to 1 or -1 to 1 if direction pin has been provided.
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
				var val = Math.Clamp(value, _pin1 != -1 ? -1.0 : 0.0, 1.0);

				if(_speed == val)
				{
					return;
				}

				if(val >= 0.0)
				{
					if(_pin1 != -1)
					{
						Controller.Write(_pin1, PinValue.Low);
					}

					_pwm.DutyCycle = val;
				}
				else
				{
					if(_pin1 != -1)
					{
						Controller.Write(_pin1, PinValue.High);
					}

					_pwm.DutyCycle = 1.0 + val;
				}

				_speed = val;
			}
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected override void Dispose(Boolean disposing)
		{
			if(disposing)
			{
				_pwm?.Dispose();
				_pwm = null;
			}

			base.Dispose(disposing);
		}
	}
}