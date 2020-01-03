using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetComDeselectVoltageLevel command
	/// </summary>
	public class SetComDeselectVoltageLevel : ISsd1327Command
	{
		/// <summary>
		/// This command sets the high voltage level of common pins, Vcomh.
		/// </summary>
		/// <param name="level">COM deselect voltage level.</param>
		public SetComDeselectVoltageLevel(Byte level = 0x05)
		{
			if(level > 0x07)
			{
				throw new ArgumentOutOfRangeException(nameof(level));
			}

			Level = level;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xBE;

		/// <summary>
		/// COM deselect voltage level.
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
