﻿using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetPreChargePeriod command
	/// </summary>
	public class SetPreChargePeriod : ISsd1306Command
	{
		/// <summary>
		/// This command is used to set the duration of the pre-charge period.
		/// The interval is counted in number of DCLK, where RESET equals 2 DCLKs.
		/// </summary>
		/// <param name="phase1Period">Phase 1 period with a range of 1-15.</param>
		/// <param name="phase2Period">Phase 2 period with a range of 1-15.</param>
		public SetPreChargePeriod(Byte phase1Period = 0x02, Byte phase2Period = 0x02)
		{
			if(!Ssd13xx.InRange(phase1Period, 0x01, 0x0F))
			{
				throw new ArgumentOutOfRangeException(nameof(phase1Period));
			}

			if(!Ssd13xx.InRange(phase2Period, 0x01, 0x0F))
			{
				throw new ArgumentOutOfRangeException(nameof(phase2Period));
			}

			Phase1Period = phase1Period;
			Phase2Period = phase2Period;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xD9;

		/// <summary>
		/// Phase 1 period with a range of 1-15.
		/// </summary>
		public Byte Phase1Period { get; }

		/// <summary>
		/// Phase 2 period with a range of 1-15.
		/// </summary>
		public Byte Phase2Period { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			var phasePeriod = (Byte)((Phase2Period << 4) | Phase1Period);
			return new Byte[] { Id, phasePeriod };
		}
	}
}
