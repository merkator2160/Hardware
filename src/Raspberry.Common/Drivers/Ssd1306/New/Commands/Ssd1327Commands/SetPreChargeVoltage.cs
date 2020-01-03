using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetPreChargeVoltage command
	/// </summary>
	public class SetPreChargeVoltage : ISsd1327Command
	{
		/// <summary>
		/// This command sets the first pre-charge voltage (phase 2) level of segment pins.
		/// </summary>
		/// <param name="level">
		/// Pre-charge voltage level.
		/// Parameter values between 0b_0000 and 0b_0111 leads to voltage values between 0.2 x Vcc and 0.613 x Vcc Volts.
		/// Parameter value 0b_1XXX leads to voltage value equals to Vcomh.
		/// </param>
		public SetPreChargeVoltage(Byte level = 0x05)
		{
			if(level > 0x08)
			{
				throw new ArgumentOutOfRangeException(nameof(level));
			}

			Level = level;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xBC;

		/// <summary>
		/// Pre-charge voltage level.
		/// </summary>
		public Byte Level { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, Level };
		}
	}
}
