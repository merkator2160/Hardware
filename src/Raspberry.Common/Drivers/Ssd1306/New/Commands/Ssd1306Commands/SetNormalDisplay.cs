using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetNormalDisplay command
	/// </summary>
	public class SetNormalDisplay : ISsd1306Command
	{
		/// <summary>
		/// This command sets the display to be normal.  Displays a RAM data of 1 indicates an ON pixel.
		/// </summary>
		public SetNormalDisplay()
		{
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xA6;

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
