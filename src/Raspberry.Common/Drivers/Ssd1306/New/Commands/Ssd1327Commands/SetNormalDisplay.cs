using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetNormalDisplay command
	/// </summary>
	public class SetNormalDisplay : ISsd1327Command
	{
		/// <summary>
		/// This command sets the display to be normal.
		/// </summary>
		public SetNormalDisplay()
		{
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xA4;

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
