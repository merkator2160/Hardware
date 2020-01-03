using System;

namespace Common.Drivers.Ssd1306.New.Commands
{
	/// <summary>
	/// Represents DeactivateScroll command
	/// </summary>
	public class DeactivateScroll : ISharedCommand
	{
		/// <summary>
		/// This command stops the motion of scrolling. After sending 2Eh command to deactivate
		/// the scrolling action, the ram data needs to be rewritten.
		/// </summary>
		public DeactivateScroll()
		{
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0x2E;

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
