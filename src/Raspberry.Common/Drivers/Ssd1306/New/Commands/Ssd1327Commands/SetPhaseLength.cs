using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Sets the length of phase 1 and 2 of segment waveform of the driver.
	/// </summary>
	public class SetPhaseLength : ISsd1327Command
	{
		/// <summary>
		/// Constructs instance of SetPhaseLength command
		/// </summary>
		/// <param name="phase1Period">Phase 1 period</param>
		/// <param name="phase2Period">Phase 2 period</param>
		public SetPhaseLength(Byte phase1Period = 0x02, Byte phase2Period = 0x02)
		{
			CheckPeriods(phase1Period, phase2Period);

			Phase1Period = phase1Period;
			Phase2Period = phase2Period;
			PhasePeriod = (Byte)((Phase2Period << 4) | Phase1Period);
		}

		/// <summary>
		/// Constructs instance of SetPhaseLength command
		/// </summary>
		/// <param name="phasePeriod">Phase period</param>
		public SetPhaseLength(Byte phasePeriod)
		{
			var phase1Period = (Byte)(phasePeriod & 0x0F);
			var phase2Period = (Byte)((phasePeriod & 0xF0) >> 4);
			CheckPeriods(phase1Period, phase2Period);

			Phase1Period = phase1Period;
			Phase2Period = phase2Period;
			PhasePeriod = phasePeriod;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xB1;

		/// <summary>
		/// Phase 1 period with a range of 1-15.
		/// </summary>
		public Byte Phase1Period { get; }

		/// <summary>
		/// Phase 2 period with a range of 1-15.
		/// </summary>
		public Byte Phase2Period { get; }

		/// <summary>
		/// Phase period.
		/// </summary>
		public Byte PhasePeriod { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, PhasePeriod };
		}

		private void CheckPeriods(Byte phase1Period, Byte phase2Period)
		{
			if(!Ssd13xx.InRange(phase1Period, 0x01, 0x0F))
			{
				throw new ArgumentOutOfRangeException(nameof(phase1Period));
			}

			if(!Ssd13xx.InRange(phase2Period, 0x01, 0x0F))
			{
				throw new ArgumentOutOfRangeException(nameof(phase2Period));
			}
		}
	}
}
