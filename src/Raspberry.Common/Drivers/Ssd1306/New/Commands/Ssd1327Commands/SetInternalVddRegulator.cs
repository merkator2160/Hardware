using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetInternalVddRegulator command
	/// </summary>
	public class SetInternalVddRegulator : ISsd1327Command
	{
		/// <summary>
		/// This command is used to enable internal Vdd regulator.
		/// </summary>
		/// <param name="enable">Represents if internal Vdd have to be enabled.</param>
		public SetInternalVddRegulator(Boolean enable)
		{
			UseInternalVdd = (Byte)(enable ? 0b_0000_0001 : 0b_0000_0000);
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xAB;

		/// <summary>
		/// The value that represent if internal or external Vdd should be used.
		/// </summary>
		public Byte UseInternalVdd { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, UseInternalVdd };
		}
	}
}
