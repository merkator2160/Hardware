using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetColumnAddress command
	/// </summary>
	public class SetColumnAddress : ISsd1327Command
	{
		/// <summary>
		/// Set column address.
		/// Start from 8th column of driver IC. This is 0th column for OLED.
		/// End at  (8 + 47)th column. Each column has 2 pixels(or segments).
		/// </summary>
		/// <param name="startAddress">Column start address with a range of 8-55.</param>
		/// <param name="endAddress">Column end address with a range of 8-55.</param>
		public SetColumnAddress(Byte startAddress = 0x08, Byte endAddress = 0x37)
		{
			if(startAddress > 0x37)
			{
				throw new ArgumentOutOfRangeException(nameof(startAddress));
			}

			if(endAddress > 0x37)
			{
				throw new ArgumentOutOfRangeException(nameof(endAddress));
			}

			StartAddress = startAddress;
			EndAddress = endAddress;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0x15;

		/// <summary>
		/// Column start address.
		/// </summary>
		public Byte StartAddress { get; set; }

		/// <summary>
		/// Column end address.
		/// </summary>
		public Byte EndAddress { get; set; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, StartAddress, EndAddress };
		}
	}
}
