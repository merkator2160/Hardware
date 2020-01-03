using System;

namespace Common.Drivers.Ssd1306.New.Commands
{
	/// <summary>
	/// Represents SetInverseDisplay command
	/// </summary>
	public class SetInverseDisplay : ISharedCommand
	{
		/// <summary>
		/// This command sets the display to be inverse.  Displays a RAM data of 0 indicates an ON pixel.
		/// </summary>
		public SetInverseDisplay()
		{
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xA7;

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
