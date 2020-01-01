using Common.Helpers;
using System;
using System.Device.I2c;

namespace Common.Drivers.Lcd
{
	/// <summary>
	/// Abstraction layer for accessing the lcd IC.
	/// </summary>
	public abstract class LcdInterfaceBase : IDisposable
	{
		private Boolean _disposed;

		/// <summary>
		/// Sends byte to LCD device
		/// </summary>
		/// <param name="value">Byte value to be sed</param>
		public abstract void SendData(Byte value);

		/// <summary>
		/// Sends command to the LCD device
		/// </summary>
		/// <param name="command">Byte representing the command</param>
		public abstract void SendCommand(Byte command);

		/// <summary>
		/// Sends data to the LCD device
		/// </summary>
		/// <param name="values">Bytes to be send to the device</param>
		public abstract void SendData(ReadOnlySpan<Byte> values);

		/// <summary>
		/// Send commands to the LCD device
		/// </summary>
		/// <param name="values">Each byte represents command to be send</param>
		public abstract void SendCommands(ReadOnlySpan<Byte> values);

		/// <summary>
		/// True if device uses 8-bits for communication, false if device uses 4-bits
		/// </summary>
		public abstract Boolean EightBitMode { get; }

		/// <summary>
		/// The command wait time multiplier for the LCD.
		/// </summary>
		/// <remarks>
		/// In order to handle controllers that might be running at a much slower clock
		/// we're exposing a multiplier for any "hard coded" waits. This can also be
		/// used to reduce the wait time when the clock runs faster or other overhead
		/// (time spent in other code) allows for more aggressive timing.
		///
		/// There is a busy signal that can be checked that could make this moot, but
		/// currently we are unable to check the signal fast enough to make gains (or
		/// even equal) going off hard timings. The busy signal also requires having a
		/// r/w pin attached.
		/// </remarks>
		public Double WaitMultiplier { get; set; } = 1.0;

		/// <summary>
		/// Wait for the device to not be busy.
		/// </summary>
		/// <param name="microseconds">Time to wait if checking busy state isn't possible/practical.</param>
		public virtual void WaitForNotBusy(Int32 microseconds)
		{
			DelayHelper.DelayMicroseconds((Int32)(microseconds * WaitMultiplier), allowThreadYield: true);

			// While we could check for the busy state it isn't currently practical. Most
			// commands need a maximum of 37μs to complete. Reading the busy flag alone takes
			// ~200μs (on the Pi) if going through the software driver. Prepping the pins and
			// reading once can take nearly a millisecond, which clearly is not going to be
			// performant.
			//
			// We might be able to dynamically introduce waits on the busy flag by measuring
			// the time to take a reading and utilizing the flag if we can check fast enough
			// relative to the requested wait time. If it takes 20μs to check and the wait time
			// is over 1000μs we may very well save significant time as the "slow" commands
			// (home and clear) can finish much faster than the time we've allocated.

			// Timings in the original HD44780 specification are based on a "typical" 270kHz
			// clock. (See page 25.) Most instructions take 3 clocks to complete. The internal
			// clock (fOSC) is documented as varying from 190-350KHz on the HD44780U and 140-450KHz
			// on the PCF2119x.
		}

		/// <summary>
		/// Enable/disable the backlight. (Will always return false if no backlight pin was provided.)
		/// </summary>
		public abstract Boolean BackLightOn { get; set; }


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		public static LcdInterfaceBase CreateI2c(Int32 address, Int32 busId = 1, Boolean uses8Bit = true)
		{
			var device = I2cDevice.Create(new I2cConnectionSettings(busId, address));
			if(uses8Bit)
				return new LcdInterfaceI2c(device);

			return new LcdInterfaceI2c4Bit(device);
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected virtual void Dispose(Boolean disposing)
		{

		}
		public void Dispose()
		{
			if(_disposed)
				return;

			Dispose(true);
			_disposed = true;
		}
	}
}
