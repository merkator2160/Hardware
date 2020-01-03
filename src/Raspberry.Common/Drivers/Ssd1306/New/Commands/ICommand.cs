using System;

namespace Common.Drivers.Ssd1306.New.Commands
{
	/// <summary>
	/// Interface for all Ssd13xx commands
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// The value that represents the command.
		/// </summary>
		Byte Id { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		Byte[] GetBytes();
	}
}
