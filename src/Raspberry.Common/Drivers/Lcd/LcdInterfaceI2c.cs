// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Common.Drivers.Lcd.Flags;
using System;
using Windows.Devices.I2c;

namespace Common.Drivers.Lcd
{
	public class LcdInterfaceI2c : LcdInterfaceBase
	{
		private readonly I2cDevice _device;


		public LcdInterfaceI2c(I2cDevice device)
		{
			_device = device;

			// While the LCD controller can be set to 4 bit mode there really isn't a way to
			// mess with that from the I2c pins as far as I know. Other drivers try to set the
			// controller up for 8 bit mode, but it appears they are doing so only because they've
			// copied existing HD44780 drivers.
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////
		public override Boolean EightBitMode => true;
		public override Boolean BackLightOn
		{
			get
			{
				// Setting the backlight on or off is not supported with 8 bit commands, according to the docs.
				return true;
			}
			set
			{
				// Ignore setting the backlight. Exceptions are not expected by user code here, as it is normal to
				// enable this during initialization, so that it is enabled whether switching it is supported or not.
			}
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////
		public override void SendCommand(Byte command)
		{
			Span<Byte> buffer = stackalloc Byte[]
			{
				0x00,
				command
			};
			_device.Write(buffer.ToArray());
		}
		public override void SendCommands(ReadOnlySpan<Byte> commands)
		{
			// There is a limit to how much data the controller can accept at once. Haven't found documentation
			// for this yet, can probably iterate a bit more on this to find a true "max". Not adding additional
			// logic like SendData as we don't expect a need to send more than a handful of commands at a time.
			if(commands.Length > 20)
			{
				throw new ArgumentOutOfRangeException(nameof(commands), "Too many commands in one request.");
			}

			Span<Byte> buffer = stackalloc Byte[commands.Length + 1];
			buffer[0] = 0x00;
			commands.CopyTo(buffer.Slice(1));
			_device.Write(buffer.ToArray());
		}
		public override void SendData(Byte value)
		{
			Span<Byte> buffer = stackalloc Byte[]
			{
				(Byte)ControlByteFlags.RegisterSelect,
				value
			};
			_device.Write(buffer.ToArray());
		}
		public override void SendData(ReadOnlySpan<Byte> values)
		{
			// There is a limit to how much data the controller can accept at once. Haven't found documentation
			// for this yet, can probably iterate a bit more on this to find a true "max". 40 was too much.
			const Int32 MaxCopy = 20;
			Span<Byte> buffer = stackalloc Byte[MaxCopy + 1];
			buffer[0] = (Byte)ControlByteFlags.RegisterSelect;
			Span<Byte> bufferData = buffer.Slice(1);

			while(values.Length > 0)
			{
				ReadOnlySpan<Byte> currentValues = values.Slice(0, values.Length > MaxCopy ? MaxCopy : values.Length);
				values = values.Slice(currentValues.Length);
				currentValues.CopyTo(bufferData);
				_device.Write(buffer.Slice(0, currentValues.Length + 1).ToArray());
			}
		}

		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected override void Dispose(Boolean disposing)
		{
			_device?.Dispose();
		}
	}
}
