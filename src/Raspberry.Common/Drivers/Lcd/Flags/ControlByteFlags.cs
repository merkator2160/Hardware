using System;

namespace Common.Drivers.Lcd.Flags
{
	[Flags]
	public enum ControlByteFlags : byte
	{
		/// <summary>
		/// When set, another control byte will follow the next data/command byte.
		/// Otherwise the last control byte will be used.
		/// </summary>
		/// <remarks>
		/// This is only relevant when sending multiple bytes of data to the register.
		/// When a new I2c transmission is made, the first byte is always assumed to
		/// be a control byte. This flag is needed if you want to flip the "RS" bit
		/// in a stream of bytes.
		/// </remarks>
		ControlByteFollows = 0b_1000_0000,

		/// <summary>
		/// When set the data register will be selected (i.e. equivalent to
		/// RS pin being high). Otherwise the instruction/command register
		/// will be updated.
		/// </summary>
		RegisterSelect = 0b_0100_0000

		// No other bits are used
	}
}