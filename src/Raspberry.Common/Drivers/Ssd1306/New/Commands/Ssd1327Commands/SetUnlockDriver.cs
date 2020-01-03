using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SetUnlockDriver command
	/// </summary>
	public class SetUnlockDriver : ISsd1327Command
	{
		/// <summary>
		/// This command sets the display to be normal.
		/// </summary>
		/// <param name="unlock">Represents if driver have to be unlocked.</param>
		public SetUnlockDriver(Boolean unlock)
		{
			SetUnlock = (Byte)(unlock ? 0b_0001_0010 : 0b_0001_0110);
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xFD;

		/// <summary>
		/// The value that represents if driver should be unlocked.
		/// </summary>
		private Byte SetUnlock { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, SetUnlock };
		}
	}
}
