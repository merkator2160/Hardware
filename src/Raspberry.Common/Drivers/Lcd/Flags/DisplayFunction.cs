// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Common.Drivers.Lcd.Flags
{
	// Command flags are found on page 24/25 of the HD4478U spec.

	[Flags]
	internal enum DisplayFunction : byte
	{
		/// <summary>
		/// When set, commands (other than <see cref="DisplayFunction"/>) are from the
		/// extended (NXP/Philips, Sitronix) instruction set. For Sitronix (ST7036) this
		/// is extended intstruction set 1.
		/// </summary>
		/// <remarks>
		/// The "H" option from the NXP datasheet. The "IS1" option from Sitronix.
		/// Not all driver ICs support this.
		/// </remarks>
		ExtendedInstructionSet = 0b_0000_0001,

		/// <summary>
		/// If set font is 5x10, otherwise font is 5x8.
		/// </summary>
		/// <remarks>
		/// The "F" option from the HD44780 datasheet.
		///
		/// The displays that supported 5x10 are extremely rare.
		///
		/// On NXP/Philips chips this is the "M" option that
		/// controls the number of display lines. When set the
		/// driver is set to 2 line x 16 characters. Otherwise
		/// the driver is set to 1 line by 32 characters.
		///
		/// For Sitronix, this is referred to as "DH" and sets
		/// double height mode.
		/// </remarks>
		Font5x10 = 0b_0000_0100,

		/// <summary>
		/// If set display is two line, otherwise display is one line.
		/// </summary>
		/// <remarks>
		/// The "N" option from the datasheet.
		///
		/// When set to one line, shifting the display right pulls
		/// address 0x4F to the 1st display position. When set to two
		/// lines, shifting right pulls 0x27 for the first line and
		/// 0x67 for the second.
		/// </remarks>
		TwoLine = 0b_0000_1000,

		/// <summary>
		/// If set display uses all eight data pins, otherwise display uses
		/// four data pins.
		/// </summary>
		/// <remarks>The "DL" option from the datasheet.</remarks>
		EightBit = 0b_0001_0000,

		/// <summary>
		/// The flag for setting display function- must be set.
		/// </summary>
		Command = 0b_0010_0000
	}
}