using System;

namespace Common.Drivers.Ssd1306.New.Commands
{
	/// <summary>
	/// Represents SetDisplayOn command
	/// </summary>
	public class SetDisplayOn : ISharedCommand
	{
		/// <summary>
		/// This command turns the OLED panel display on.
		/// </summary>
		public SetDisplayOn()
		{
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xAF;

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
