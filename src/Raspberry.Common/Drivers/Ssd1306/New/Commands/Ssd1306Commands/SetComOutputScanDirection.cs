using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetComOutputScanDirection command
	/// </summary>
	public class SetComOutputScanDirection : ISsd1306Command
	{
		/// <summary>
		/// This command sets the scan direction of the COM output, allowing layout flexibility
		/// in the OLED module design. Additionally, the display will show once this command is
		/// issued. For example, if this command is sent during normal display then the graphic
		/// display will be vertically flipped immediately.
		/// </summary>
		/// <param name="normalMode">
		/// Scan from COM0 to COM[N –1] when TRUE.
		/// Scan from COM[N - 1] to COM0 when FALSE.
		/// </param>
		public SetComOutputScanDirection(Boolean normalMode = true)
		{
			NormalMode = normalMode;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => (Byte)(NormalMode ? 0xC0 : 0xC8);

		/// <summary>
		/// Scan from COM0 to COM[N –1] when TRUE.
		/// Scan from COM[N - 1] to COM0 when FALSE.
		/// </summary>
		public Boolean NormalMode { get; set; }

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
