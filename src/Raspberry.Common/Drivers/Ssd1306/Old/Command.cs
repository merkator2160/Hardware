using System;

namespace Common.Drivers.Ssd1306.Old
{
	/// <summary>
	/// Display commands. See the data sheet for details on commands: http://www.adafruit.com/datasheets/SSD1306.pdf
	/// </summary>
	public static class Command
	{
		public static readonly Byte[] Off = { 0xAE };                       // Turns the display off
		public static readonly Byte[] On = { 0xAF };                        // Turns the display on
		public static readonly Byte[] ChargePumpOn = { 0x8D, 0x14 };        // Turn on internal charge pump to supply power to display
		public static readonly Byte[] MemAddrMode = { 0x20, 0x00 };         // Horizontal memory mode
		public static readonly Byte[] SegRemap = { 0xA1 };                  // Remaps the segments, which has the effect of mirroring the display horizontally
		public static readonly Byte[] ComScanDir = { 0xC8 };                // Set the COM scan direction to inverse, which flips the screen vertically
		public static readonly Byte[] ResetColAddr = { 0x21, 0x00, 0x7F };  // Reset the column address pointer
		public static readonly Byte[] ResetPageAddr = { 0x22, 0x00, 0x07 }; // Reset the page address pointer
	}
}