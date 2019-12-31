using System;

namespace Common.Drivers.Lcd.Flags
{
	[Flags]
	internal enum DisplayEntryMode : byte
	{
		/// <summary>
		/// Enabled to shift the display left when <see cref="Increment"/> is enabled
		/// or right if <see cref="Increment"/> is disabled.
		/// </summary>
		/// <remarks>The "S" option from the datasheet.</remarks>
		DisplayShift = 0b_0001,

		/// <summary>
		/// Set to increment the CGRAM/DDRAM address by 1 when a character code is
		/// written into or read from and moves the cursor to the right. Disabling
		/// decrements and moves the cursor to the left.
		/// </summary>
		/// <remarks>The "I/D" option from the datasheet.</remarks>
		Increment = 0b_0010,

		/// <summary>
		/// The flag for entry mode- must be set.
		/// </summary>
		Command = 0b_0100,
	}
}