using Common.Drivers.Ssd1306.New.Commands;
using System;
using System.Device.I2c;

namespace Common.Drivers.Ssd1306.New
{
	/// <summary>
	/// Represents base class for SSD13xx OLED displays
	/// </summary>
	public abstract class Ssd13xx : IDisposable
	{
		// Multiply of screen resolution plus single command byte.
		private const Int32 DefaultBufferSize = 48 * 96 + 1;
		private Byte[] _genericBuffer;

		/// <summary>
		/// Underlying I2C device
		/// </summary>
		protected I2cDevice _i2cDevice;

		/// <summary>
		/// Constructs instance of Ssd13xx
		/// </summary>
		/// <param name="i2cDevice">I2C device used to communicate with the device</param>
		/// <param name="bufferSize">Command buffer size</param>
		public Ssd13xx(I2cDevice i2cDevice, Int32 bufferSize = DefaultBufferSize)
		{
			_genericBuffer = new Byte[bufferSize];
			_i2cDevice = i2cDevice;
		}

		/// <summary>
		/// Verifies value is within a specific range.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="start">Starting value of range.</param>
		/// <param name="end">Ending value of range.</param>
		/// <returns>Determines if value is within range.</returns>
		internal static Boolean InRange(UInt32 value, UInt32 start, UInt32 end)
		{
			return (value - start) <= (end - start);
		}

		/// <summary>
		/// Send a command to the display controller.
		/// </summary>
		/// <param name="command">The command to send to the display controller.</param>
		public abstract void SendCommand(ISharedCommand command);

		/// <summary>
		/// Send data to the display controller.
		/// </summary>
		/// <param name="data">The data to send to the display controller.</param>
		public virtual void SendData(Byte[] data)
		{
			if(data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var writeBuffer = SliceGenericBuffer(data.Length + 1);

			writeBuffer[0] = 0x40; // Control byte.
			data.CopyTo(writeBuffer.Slice(1));
			_i2cDevice.Write(writeBuffer);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			_i2cDevice?.Dispose();
			_i2cDevice = null;
		}

		/// <summary>
		/// Acquires span of specific length pointing to the command buffer.
		/// If length of the command buffer is too small it will be reallocated.
		/// </summary>
		/// <param name="length">Requested length</param>
		/// <returns>Span of bytes pointing to the command buffer</returns>
		protected Span<Byte> SliceGenericBuffer(Int32 length)
		{
			return SliceGenericBuffer(0, length);
		}

		/// <summary>
		/// Acquires span of specific length at specific position in command buffer.
		/// If length of the command buffer is too small it will be reallocated.
		/// </summary>
		/// <param name="start">Start index of the requested span</param>
		/// <param name="length">Requested length</param>
		/// <returns>Span of bytes pointing to the command buffer</returns>
		protected Span<Byte> SliceGenericBuffer(Int32 start, Int32 length)
		{
			if(_genericBuffer.Length < length)
			{
				var newBuffer = new Byte[_genericBuffer.Length * 2];
				_genericBuffer.CopyTo(newBuffer, 0);
				_genericBuffer = newBuffer;
			}

			return _genericBuffer.AsSpan(start, length);
		}
	}
}
