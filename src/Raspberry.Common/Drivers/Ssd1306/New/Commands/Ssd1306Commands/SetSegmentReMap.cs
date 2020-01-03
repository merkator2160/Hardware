using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetSegmentReMap command
	/// </summary>
	public class SetSegmentReMap : ISsd1306Command
	{
		/// <summary>
		/// This command changes the mapping between the display data column address and the segment driver.
		/// It allows flexibility in OLED module design. This command only affects subsequent data input.
		/// Data already stored in GDDRAM will have no changes.
		/// </summary>
		/// <param name="columnAddress127">Column address 0 is mapped to SEG0 when FALSE.
		/// Column address 127 is mapped to SEG0 when TRUE.</param>
		public SetSegmentReMap(Boolean columnAddress127 = false)
		{
			ColumnAddress127 = columnAddress127;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => (Byte)(ColumnAddress127 ? 0xA1 : 0xA0);

		/// <summary>
		/// Column Address 127.  Column address 127 is mapped to SEG0 when FALSE.
		/// Column address 127 is mapped to SEG0 when TRUE.
		/// </summary>
		public Boolean ColumnAddress127 { get; set; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id };
		}
	}
}
