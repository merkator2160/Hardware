using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetVcomhDeselectLevel command
	/// </summary>
	public class SetVcomhDeselectLevel : ISsd1306Command
	{
		/// <summary>
		/// This command adjusts the VCOMH regulator output.
		/// </summary>
		/// <param name="level">Vcomh deselect level.</param>
		public SetVcomhDeselectLevel(DeselectLevel level = DeselectLevel.Vcc0_77)
		{
			Level = level;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xDB;

		/// <summary>
		/// Vcomh deselect level.
		/// </summary>
		public DeselectLevel Level { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, (Byte)Level };
		}

		/// <summary>
		/// Deselect level
		/// </summary>
		public enum DeselectLevel
		{
			/// <summary>
			/// ~0.65 x Vcc.
			/// </summary>
			Vcc0_65 = 0x00,

			/// <summary>
			/// ~0.77 x Vcc.  Default value after reset.
			/// </summary>
			Vcc0_77 = 0x20,

			/// <summary>
			/// ~0.83 x Vcc.
			/// </summary>
			Vcc0_83 = 0x30,

			/// <summary>
			/// ~1.00 x Vcc.
			/// </summary>
			Vcc1_00 = 0x40 // Not on option in datasheet, but was noted to give maximum brightness.
		}
	}
}
