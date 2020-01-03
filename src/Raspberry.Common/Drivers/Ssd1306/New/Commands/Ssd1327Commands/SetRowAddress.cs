using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetRowAddress command
	/// </summary>
	public class SetRowAddress : ISsd1327Command
	{
		/// <summary>
		/// Set row address
		/// </summary>
		/// <param name="startAddress">Column start address with a range of 0-95.</param>
		/// <param name="endAddress">Column end address with a range of 0-95.</param>
		public SetRowAddress(Byte startAddress = 0x00, Byte endAddress = 0x5f)
		{
			if(startAddress > 0x5f)
			{
				throw new ArgumentOutOfRangeException(nameof(startAddress));
			}

			if(endAddress > 0x5f)
			{
				throw new ArgumentOutOfRangeException(nameof(endAddress));
			}

			StartAddress = startAddress;
			EndAddress = endAddress;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0x75;

		/// <summary>
		/// Row start address.
		/// </summary>
		public Byte StartAddress { get; set; }

		/// <summary>
		/// Row end address.
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
