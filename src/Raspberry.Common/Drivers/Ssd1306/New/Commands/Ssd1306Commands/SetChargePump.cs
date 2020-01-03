using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetChargePump command
	/// </summary>
	public class SetChargePump : ISsd1306Command
	{
		/// <summary>
		/// This command controls the switching capacitor regulator circuit.
		/// </summary>
		/// <param name="enableChargePump">Determines if charge pump is enabled while the display is on.</param>
		public SetChargePump(Boolean enableChargePump = false)
		{
			EnableChargePump = enableChargePump;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0x8D;

		/// <summary>
		/// Determines if charge pump is enabled while the display is on.
		/// </summary>
		public Boolean EnableChargePump { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			Byte enableChargePump = 0x10;

			if(EnableChargePump)
			{
				enableChargePump = 0x14;
			}

			return new Byte[] { Id, enableChargePump };
		}
	}
}
