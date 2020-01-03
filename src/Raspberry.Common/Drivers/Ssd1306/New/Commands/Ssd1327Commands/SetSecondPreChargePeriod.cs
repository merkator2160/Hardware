using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetSecondPreChargePeriod command
	/// </summary>
	public class SetSecondPreChargePeriod : ISsd1327Command
	{
		/// <summary>
		/// This command is used to set the phase 3 second pre-charge period.
		/// </summary>
		/// <param name="period">Second pre-charge period.</param>
		public SetSecondPreChargePeriod(Byte period = 0x04)
		{
			if(period > 0x0F)
			{
				throw new ArgumentOutOfRangeException(nameof(period));
			}

			Period = period;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xB6;

		/// <summary>
		/// Second Pre-charge period.
		/// </summary>
		public Byte Period { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, Period };
		}
	}
}
