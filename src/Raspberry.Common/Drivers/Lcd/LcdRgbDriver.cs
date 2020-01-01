using Common.Drivers.Lcd.Flags;
using System;
using System.Device.I2c;
using System.Drawing;

namespace Common.Drivers.Lcd
{
	public class LcdRgbDriver : Hd44780Driver
	{
		private readonly I2cDevice _device;

		private Color _currentColor;
		private Boolean _backLightOn = true;


		public LcdRgbDriver(Size size, Int32 address, I2cDevice device) : base(size, LcdInterfaceBase.CreateI2c(address))
		{
			_device = device;

			InitRgb();
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Enable/disable the backlight.
		/// </summary>
		public override Boolean BackLightOn
		{
			get => _backLightOn;
			set
			{
				ForceSetBackLightColor(value ? _currentColor : Color.Black);
				_backLightOn = value;
			}
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Sets register for RGB backlight.
		/// </summary>
		/// <param name="addr">The register address.</param>
		/// <param name="value">The register value.</param>
		private void SetRgbRegister(RgbRegisters addr, Byte value)
		{
			Span<Byte> dataToSend = stackalloc Byte[]
			{
				(Byte)addr,
				value
			};
			_device.Write(dataToSend);
		}

		/// <summary>
		/// Sets the backlight color without any checks.
		/// </summary>
		/// <param name="color">The color to set.</param>
		private void ForceSetBackLightColor(Color color)
		{
			SetRgbRegister(RgbRegisters.REG_RED, color.R);
			SetRgbRegister(RgbRegisters.REG_GREEN, color.G);
			SetRgbRegister(RgbRegisters.REG_BLUE, color.B);
		}

		/// <summary>
		/// Initializes RGB device.
		/// </summary>
		private void InitRgb()
		{
			// backlight init
			SetRgbRegister(RgbRegisters.REG_MODE1, 0);

			// set LEDs controllable by both PWM and GRPPWM registers
			SetRgbRegister(RgbRegisters.REG_LEDOUT, 0xFF);

			// set MODE2 values
			// 0010 0000 -> 0x20  (DMBLNK to 1, ie blinky mode)
			SetRgbRegister(RgbRegisters.REG_MODE2, 0x20);

			SetBackLightColor(Color.White);
		}

		/// <summary>
		/// Sets the backlight color.
		/// The action will be ignored in case of the backlight is disabled.
		/// </summary>
		/// <param name="color">The color to set.</param>
		public void SetBackLightColor(Color color)
		{
			if(!BackLightOn)
			{
				return;
			}

			ForceSetBackLightColor(color);
			_currentColor = color;
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected override void Dispose(Boolean disposing)
		{
			_device?.Dispose();
			base.Dispose(disposing);
		}
	}
}
