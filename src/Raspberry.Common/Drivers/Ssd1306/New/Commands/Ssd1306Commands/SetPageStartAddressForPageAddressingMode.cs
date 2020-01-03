using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetPageStartAddressForPageAddressingMode command
	/// </summary>
	public class SetPageStartAddressForPageAddressingMode : ISsd1306Command
	{
		/// <summary>
		/// This command positions the page start address from 0 to 7 in GDDRAM under Page Addressing Mode.
		/// </summary>
		/// <param name="startAddress">Page start address with a range of 0-7.</param>
		public SetPageStartAddressForPageAddressingMode(PageAddress startAddress = PageAddress.Page0)
		{
			StartAddress = startAddress;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => (Byte)(0xB0 + StartAddress);

		/// <summary>
		/// Page start address with a range of 0-7.
		/// </summary>
		public PageAddress StartAddress { get; }

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
