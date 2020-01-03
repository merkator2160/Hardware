using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetLowerColumnStartAddressForPageAddressingMode command
	/// </summary>
	public class SetLowerColumnStartAddressForPageAddressingMode : ISsd1306Command
	{
		/// <summary>
		/// This command specifies the lower nibble of the 8-bit column start address for the display
		/// data RAM under Page Addressing Mode. The column address will be incremented by each data access.
		/// This command is only for page addressing mode.
		/// </summary>
		/// <param name="lowerColumnStartAddress">Lower column start address with a range of 0-15.</param>
		public SetLowerColumnStartAddressForPageAddressingMode(Byte lowerColumnStartAddress = 0x00)
		{
			if(lowerColumnStartAddress > 0x0F)
			{
				throw new ArgumentOutOfRangeException(nameof(lowerColumnStartAddress));
			}

			LowerColumnStartAddress = lowerColumnStartAddress;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => LowerColumnStartAddress;

		/// <summary>
		/// Lower column start address with a range of 0-15.
		/// </summary>
		public Byte LowerColumnStartAddress { get; }

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
