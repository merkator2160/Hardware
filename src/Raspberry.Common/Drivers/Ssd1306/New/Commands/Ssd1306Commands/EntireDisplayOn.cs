using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents EntireDisplayOn command
	/// </summary>
	public class EntireDisplayOn : ISsd1306Command
	{
		/// <summary>
		/// This command turns the entire display on or off.
		/// </summary>
		/// <param name="entireDisplay">Resume to RAM content display when FALSE and turns entire dislay on when TRUE.</param>
		public EntireDisplayOn(Boolean entireDisplay)
		{
			EntireDisplay = entireDisplay;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => (Byte)(EntireDisplay ? 0xA5 : 0xA4);

		/// <summary>
		/// Resume to RAM content display when FALSE and turns entire dislay on when TRUE.
		/// </summary>
		public Boolean EntireDisplay { get; }

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
