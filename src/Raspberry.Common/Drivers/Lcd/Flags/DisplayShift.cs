using System;

namespace Common.Drivers.Lcd.Flags
{
	[Flags]
	internal enum DisplayShift : byte
	{
		/// <summary>
		/// When set shifts right, otherwise shifts left.
		/// </summary>
		/// <remarks>The "R/L" option from the datasheet.</remarks>
		Right = 0b_0000_0100,

		/// <summary>
		/// When set shifts the display when data is entered, otherwise shifts the cursor.
		/// </summary>
		/// <remarks>The "S/C" option from the datasheet.</remarks>
		Display = 0b_0000_1000,

		/// <summary>
		/// The flag for display and cursor shift- must be set.
		/// </summary>
		Command = 0b_0001_0000
	}
}