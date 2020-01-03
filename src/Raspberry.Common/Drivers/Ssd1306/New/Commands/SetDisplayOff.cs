using System;

namespace Common.Drivers.Ssd1306.New.Commands
{
	/// <summary>
	/// Represents SetDisplayOff command
	/// </summary>
	public class SetDisplayOff : ISharedCommand
	{
		/// <summary>
		/// This command turns the OLED panel display off.
		/// </summary>
		public SetDisplayOff()
		{
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xAE;

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
