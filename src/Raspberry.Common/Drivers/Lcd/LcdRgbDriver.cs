using Common.Drivers.Lcd.Flags;
using System;
using System.Drawing;
using Windows.Devices.I2c;

namespace Common.Drivers.Lcd
{
	/// <summary>
	/// Supports I2c LCDs with I2c RGB backlight, such as the Grove - LCD RGB Backlight (i.e. 16x2 LCD character display with RGB backlight).
	/// </summary>
	/// <remarks>
	/// This implementation was drawn from numerous libraries such as Grove_LCD_RGB_Backlight.
	/// </remarks>
	public class LcdRgbDriver : Hd44780Driver
	{
		private readonly I2cDevice _rgbDevice;

		private Color _currentColor;
		private Boolean _backlightOn = true;



		/// <summary>
		/// Initializes a new HD44780 LCD controller.
		/// </summary>
		/// <param name="size">Size of the device in characters. Usually 16x2 or 20x4.</param>
		/// <param name="lcdDevice">The I2C device to control LCD display.</param>
		/// <param name="rgbDevice">The I2C device to control RGB backlight.</param>
		public LcdRgbDriver(Size size, Int32 address, I2cDevice rgbDevice) : base(size, LcdInterfaceBase.CreateI2c(address, true))
		{
			_rgbDevice = rgbDevice;

			InitRgb();
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Enable/disable the backlight.
		/// </summary>
		public override Boolean BackLightOn
		{
			get => _backlightOn;
			set
			{
				ForceSetBacklightColor(value ? _currentColor : Color.Black);
				_backlightOn = value;
			}
		}

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
			_rgbDevice.Write(dataToSend.ToArray());
		}

		/// <summary>
		/// Sets the backlight color without any checks.
		/// </summary>
		/// <param name="color">The color to set.</param>
		private void ForceSetBacklightColor(Color color)
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

			SetBacklightColor(Color.White);
		}

		/// <summary>
		/// Sets the backlight color.
		/// The action will be ignored in case of the backlight is disabled.
		/// </summary>
		/// <param name="color">The color to set.</param>
		public void SetBacklightColor(Color color)
		{
			if(!BackLightOn)
			{
				return;
			}

			ForceSetBacklightColor(color);
			_currentColor = color;
		}

		/// <inheritdoc/>
		protected override void Dispose(Boolean disposing)
		{
			_rgbDevice?.Dispose();
			base.Dispose(disposing);
		}
	}
}
