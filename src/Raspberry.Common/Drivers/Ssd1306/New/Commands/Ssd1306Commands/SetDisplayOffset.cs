using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetDisplayOffset command
	/// </summary>
	public class SetDisplayOffset : ISsd1306Command
	{
		/// <summary>
		/// This command specifies the mapping of the display start line to one of COM0-COM63
		/// (assuming that COM0 is the display start line then the display start line register is equal to 0).
		/// </summary>
		/// <param name="displayOffset">Display offset with a range of 0-63.</param>
		public SetDisplayOffset(Byte displayOffset = 0x00)
		{
			if(displayOffset > 0x3F)
			{
				throw new ArgumentOutOfRangeException(nameof(displayOffset));
			}

			DisplayOffset = displayOffset;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xD3;

		/// <summary>
		/// Display offset with a range of 0-63.
		/// </summary>
		public Byte DisplayOffset { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, DisplayOffset };
		}
	}
}
