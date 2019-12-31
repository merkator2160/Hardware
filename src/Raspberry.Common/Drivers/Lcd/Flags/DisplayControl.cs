using System;

namespace Common.Drivers.Lcd.Flags
{
	[Flags]
	internal enum DisplayControl : byte
	{
		/// <summary>
		/// Set for enabling cursor blinking.
		/// </summary>
		/// <remarks>The "B" option from the datasheet.</remarks>
		BlinkOn = 0b_0001,

		/// <summary>
		/// Set for enabling the cursor.
		/// </summary>
		/// <remarks>The "C" option from the datasheet.</remarks>
		CursorOn = 0b_0010,

		/// <summary>
		/// Set for enabling the entire display.
		/// </summary>
		/// <remarks>The "D" option from the datasheet.</remarks>
		DisplayOn = 0b_0100,

		/// <summary>
		/// The flag for display control- must be set.
		/// </summary>
		Command = 0b_1000
	}
}