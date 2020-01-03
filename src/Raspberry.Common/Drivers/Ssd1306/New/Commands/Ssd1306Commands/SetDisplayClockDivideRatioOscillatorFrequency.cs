using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetDisplayClockDivideRatioOscillatorFrequency command
	/// </summary>
	public class SetDisplayClockDivideRatioOscillatorFrequency : ISsd1306Command
	{
		/// <summary>
		/// This command sets the divide ratio to generate DCLK (Display Clock) from CLK and
		/// programs the oscillator frequency Fosc that is the source of CLK if CLS pin is pulled high.
		/// </summary>
		/// <param name="displayClockDivideRatio">Display clock divide ratio with a range of 0-15.</param>
		/// <param name="oscillatorFrequency">Oscillator frequency with a range of 0-15.</param>
		public SetDisplayClockDivideRatioOscillatorFrequency(Byte displayClockDivideRatio = 0x00, Byte oscillatorFrequency = 0x08)
		{
			if(displayClockDivideRatio > 0x0F)
			{
				throw new ArgumentOutOfRangeException(nameof(displayClockDivideRatio));
			}

			if(oscillatorFrequency > 0x0F)
			{
				throw new ArgumentOutOfRangeException(nameof(oscillatorFrequency));
			}

			DisplayClockDivideRatio = displayClockDivideRatio;
			OscillatorFrequency = oscillatorFrequency;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xD5;

		/// <summary>
		/// Display clock divide ratio with a range of 0-15.
		/// </summary>
		public Byte DisplayClockDivideRatio { get; }

		/// <summary>
		/// Oscillator frequency with a range of 0-15.
		/// </summary>
		public Byte OscillatorFrequency { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			var displayClockDivideRatioOscillatorFrequency = (Byte)((OscillatorFrequency << 4) | DisplayClockDivideRatio);
			return new Byte[] { Id, displayClockDivideRatioOscillatorFrequency };
		}
	}
}
