using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetSecondPreChargeVsl command
	/// </summary>
	public class SetSecondPreChargeVsl : ISsd1327Command
	{
		/// <summary>
		/// This command sets the first pre-charge voltage (phase 2) level of segment pins.
		/// </summary>
		/// <param name="secondPrecharge">Enable/disable second precharge.</param>
		/// <param name="externalVsl"> Switch between internal and external VSL.</param>
		public SetSecondPreChargeVsl(Boolean secondPrecharge = false, Boolean externalVsl = false)
		{
			Config = (Byte)(secondPrecharge ? 0b_0110_0010 : 0b_0110_0000);
			Config |= (Byte)(externalVsl ? 0b_0110_0001 : 0b_0110_0000);
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xD5;

		/// <summary>
		/// The value that represents configuration
		/// </summary>
		public Byte Config { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, Config };
		}
	}
}
