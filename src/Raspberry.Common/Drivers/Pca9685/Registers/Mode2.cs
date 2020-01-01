
using System;

namespace Common.Drivers.Pca9685.Registers
{
	[Flags]
	internal enum Mode2 : byte
	{
		INVRT = 0b00010000, // Bit 4
		OCH = 0b00001000, // Bit 3
		OUTDRV = 0b00000100 // Bit 2
	}
}
