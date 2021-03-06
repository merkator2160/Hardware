﻿using System;

namespace Common.Drivers.Ssd1306.New.Commands
{
	/// <summary>
	/// Represents SetMultiplexRatio command
	/// </summary>
	public class SetMultiplexRatio : ISharedCommand
	{
		/// <summary>
		/// This command switches the default 63 multiplex mode to any multiplex ratio, ranging from 15 to 127.
		/// The output pads COM0-COM63 will be switched to the corresponding COM signal.
		/// </summary>
		/// <param name="multiplexRatio">Multiplex ratio with a range of 15-127.</param>
		public SetMultiplexRatio(Byte multiplexRatio = 63)
		{
			if(!Ssd13xx.InRange(multiplexRatio, 0x0F, 0x7F))
			{
				throw new ArgumentException("The multiplex ratio is invalid.", nameof(multiplexRatio));
			}

			MultiplexRatio = multiplexRatio;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xA8;

		/// <summary>
		/// Multiplex ratio with a range of 15-63.
		/// </summary>
		public Byte MultiplexRatio { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			return new Byte[] { Id, MultiplexRatio };
		}
	}
}
