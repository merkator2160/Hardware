// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Windows.Devices.I2c;

namespace Common.Drivers.Lcd
{
	public class LcdInterfaceI2c4Bit : LcdInterfaceBase
	{
		private const Byte ENABLE = 0b0000_0100;
		private const Byte READWRITE = 0b0000_0010;
		private const Byte REGISTERSELECT = 0b0000_0001;

		private const Byte LCD_BACKLIGHT = 0x08;
		private const Byte LCD_FUNCTIONSET = 0x20;
		private const Byte LCD_DISPLAYCONTROL = 0x08;
		private const Byte LCD_CLEARDISPLAY = 0x01;
		private const Byte LCD_DISPLAYON = 0x04;
		private const Byte LCD_2LINE = 0x08;
		private const Byte LCD_5x8DOTS = 0x00;
		private const Byte LCD_ENTRYMODESET = 0x04;
		private const Byte LCD_ENTRYLEFT = 0x02;
		private const Byte LCD_4BITMODE = 0x00;

		private readonly I2cDevice _device;
		private Boolean _backLightOn;

		public LcdInterfaceI2c4Bit(I2cDevice device)
		{
			_device = device;
			_backLightOn = true;
			InitDisplay();
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////
		public override Boolean EightBitMode => false;
		public override Boolean BackLightOn
		{
			get
			{
				return _backLightOn;
			}
			set
			{
				_backLightOn = value;
				// Need to send a command to make this happen immediately.
				SendCommand(0);
			}
		}
		private Byte BackLightFlag
		{
			get
			{
				if(BackLightOn)
				{
					return LCD_BACKLIGHT;
				}
				else
				{
					return 0;
				}
			}
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////
		private void InitDisplay()
		{
			// This sequence (copied from a python example) completely resets the display (if it was
			// previously erroneously used with 8 bit access, it may not return to normal operation otherwise)
			SendCommandAndWait(0x3);
			SendCommandAndWait(0x3);
			SendCommandAndWait(0x3);
			SendCommandAndWait(0x2);
			SendCommandAndWait(LCD_FUNCTIONSET | LCD_2LINE | LCD_5x8DOTS | LCD_4BITMODE);
			SendCommandAndWait(LCD_DISPLAYCONTROL | LCD_DISPLAYON);
			SendCommandAndWait(LCD_CLEARDISPLAY);
			SendCommandAndWait(LCD_ENTRYMODESET | LCD_ENTRYLEFT);
		}
		private void SendCommandAndWait(Byte command)
		{
			// Must not run the init sequence to fast or undefined behavior may occur
			SendCommand(command);
			Thread.Sleep(1);
		}
		public override void SendCommand(Byte command)
		{
			Write4Bits((Byte)(0x00 | (command & 0xF0)));
			Write4Bits((Byte)(0x00 | ((command << 4) & 0xF0)));
		}
		private void Write4Bits(Byte command)
		{
			_device.Write(new Byte[] { (Byte)(command | ENABLE | BackLightFlag) });
			_device.Write(new Byte[] { (Byte)((command & ~ENABLE) | BackLightFlag) });
		}
		public override void SendCommands(ReadOnlySpan<Byte> commands)
		{
			foreach(var c in commands)
			{
				SendCommand(c);
			}
		}
		public override void SendData(Byte value)
		{
			Write4Bits((Byte)(REGISTERSELECT | (value & 0xF0)));
			Write4Bits((Byte)(REGISTERSELECT | ((value << 4) & 0xF0)));
		}
		public override void SendData(ReadOnlySpan<Byte> values)
		{
			foreach(var c in values)
			{
				Write4Bits((Byte)(REGISTERSELECT | (c & 0xF0)));
				Write4Bits((Byte)(REGISTERSELECT | ((c << 4) & 0xF0)));
			}
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected override void Dispose(Boolean disposing)
		{
			_device?.Dispose();
		}
	}
}