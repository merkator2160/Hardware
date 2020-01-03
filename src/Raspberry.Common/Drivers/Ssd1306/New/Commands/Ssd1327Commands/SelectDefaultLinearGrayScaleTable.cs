using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands
{
	/// <summary>
	/// Represents SelectDefaultLinearGrayScaleTable command
	/// </summary>
	public class SelectDefaultLinearGrayScaleTable : ISsd1327Command
	{
		/// <summary>
		/// This command reloads the preset linear Gray Scale table.
		/// </summary>
		public SelectDefaultLinearGrayScaleTable()
		{
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xB9;

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id };
		}
	}
}
