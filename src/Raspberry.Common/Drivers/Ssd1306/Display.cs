using Common.Models.Exceptions;
using System;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Common.Drivers.Ssd1306
{
	/// <summary>
	/// This driver is intended to be used with SSD1306 based OLED displays connected via I2c
	/// Such as http://amzn.to/1Urepy1, http://amzn.to/1VG54aU and http://www.adafruit.com/product/938 
	/// It will require four wires to the display.  Power (VCC), Ground, SCL and SDA (I2C)
	/// 
	/// For a Raspberry Pi this is:
	/// VCC -&gt; Pi Pin 1 (3.3v)
	/// Ground -&gt; Pi Pin 6
	/// SDA -&gt; Pi Pin 3
	/// SCL -&gt; Pi Pin 5
	/// 
	/// For DIYMall branded OLEDs the I2C address is 0x3C.  It may vary by manufacturer and can be changed down below.
	/// </summary>
	public class Display : IDisposable
	{
		private const UInt32 _screenWidthPx = 128;                         // Number of horizontal pixels on the display
		private const UInt32 _screenHeightPx = 64;                         // Number of vertical pixels on the display
		private const UInt32 _screenHeightPixels = _screenHeightPx / 8;    // The vertical pixels on this display are arranged into 'pages' of 8 pixels each
		private readonly Byte[,] _displayBuffer = new Byte[_screenWidthPx, _screenHeightPixels];                     // A local buffer we use to store graphics data for the screen

		// Definitions for I2C
		private const Byte _defaultAddress = 0x3C;
		private const String _i2cControllerName = "I2C1";
		private readonly Byte _address;
		private I2cDevice _displayI2c;


		public Display() : this(_defaultAddress, false)
		{

		}
		public Display(Byte address, Boolean proceedOnFail)
		{
			_address = address;

			InitI2cDevice(proceedOnFail);
			InitDisplay(proceedOnFail);
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Refresh()
		{
			SendCommand(Command.ResetColAddr);          // Reset the column address pointer back to 0
			SendCommand(Command.ResetPageAddr);         // Reset the page address pointer back to 0
			SendData(Serialize(_displayBuffer));         // Send the data over i2c
		}
		public void WriteLine(String text, UInt32 row)
		{
			WriteLine(text, row, 0);
		}

		/// <summary>
		/// DESCRIPTION: Writes a string to the display screen buffer (DisplayUpdate() needs to be called subsequently to output the buffer to the screen)
		/// INPUTS:
		/// 
		/// Line:      The string we want to render. In this sample, special characters like tabs and newlines are not supported.
		/// Col:       The horizontal column we want to start drawing at. This is equivalent to the 'X' axis pixel position.
		/// Row:       The vertical row we want to write to. The screen is divided up into 4 rows of 16 pixels each, so valid values for Row are 0,1,2,3.
		///
		/// RETURN VALUE:
		/// None. We simply return when we encounter characters that are out-of-bounds or aren't available in the font.
		/// </summary>
		public void WriteLine(String text, UInt32 row, UInt32 col)
		{
			foreach(var character in text)
			{
				var charWidth = WriteChar(character, row, col);
				col += charWidth;   /* Increment the column so we can track where to write the next character   */
				if(charWidth == 0) /* Quit if we encounter a character that couldn't be printed                */
				{
					return;
				}
			}
		}

		/// <summary>
		/// DESCRIPTION: Writes one character to the display screen buffer (DisplayUpdate() needs to be called subsequently to output the buffer to the screen)
		/// INPUTS:
		/// 
		/// Character: The character we want to draw. In this sample, special characters like tabs and newlines are not supported.
		/// Col:       The horizontal column we want to start drawing at. This is equivalent to the 'X' axis pixel position.
		/// Row:       The vertical row we want to write to. The screen is divided up into 4 rows of 16 pixels each, so valid values for Row are 0,1,2,3.
		/// 
		/// RETURN VALUE:
		/// We return the number of horizontal pixels used. This value is 0 if Row/Col are out-of-bounds, or if the character isn't available in the font.
		/// </summary>
		public UInt32 WriteChar(Char character, UInt32 row, UInt32 col)
		{
			// Check that we were able to find the font corresponding to our character
			var CharDescriptor = DisplayFontTable.GetCharacterDescriptor(character);
			if(CharDescriptor == null)
			{
				return 0;
			}

			// Make sure we're drawing within the boundaries of the screen buffer
			var MaxRowValue = (_screenHeightPixels / DisplayFontTable.FontHeightBytes) - 1;
			var MaxColValue = _screenWidthPx;
			if(row > MaxRowValue)
			{
				return 0;
			}
			if((col + CharDescriptor.CharacterWidthPx + DisplayFontTable.FontCharSpacing) > MaxColValue)
			{
				return 0;
			}

			UInt32 CharDataIndex = 0;
			var StartPage = row * 2;                                              //0
			var EndPage = StartPage + CharDescriptor.CharacterHeightBytes;        //2
			var StartCol = col;
			var EndCol = StartCol + CharDescriptor.CharacterWidthPx;
			UInt32 CurrentPage = 0;
			UInt32 CurrentCol = 0;

			/* Copy the character image into the display buffer */
			for(CurrentPage = StartPage; CurrentPage < EndPage; CurrentPage++)
			{
				for(CurrentCol = StartCol; CurrentCol < EndCol; CurrentCol++)
				{
					_displayBuffer[CurrentCol, CurrentPage] = CharDescriptor.CharacterData[CharDataIndex];
					CharDataIndex++;
				}
			}

			/* Pad blank spaces to the right of the character so there exists space between adjacent characters */
			for(CurrentPage = StartPage; CurrentPage < EndPage; CurrentPage++)
			{
				for(; CurrentCol < EndCol + DisplayFontTable.FontCharSpacing; CurrentCol++)
				{
					_displayBuffer[CurrentCol, CurrentPage] = 0x00;
				}
			}

			/* Return the number of horizontal pixels used by the character */
			return CurrentCol - StartCol;
		}
		public UInt32 WriteImage(DisplayImage img, UInt32 row, UInt32 col)
		{
			/* Make sure we're drawing within the boundaries of the screen buffer */
			var MaxRowValue = (_screenHeightPixels / img.ImageHeightBytes) - 1;
			var MaxColValue = _screenWidthPx;
			if(row > MaxRowValue)
			{
				return 0;
			}

			if((col + img.ImageWidthPx + DisplayFontTable.FontCharSpacing) > MaxColValue)
			{
				return 0;
			}

			UInt32 CharDataIndex = 0;
			var StartPage = row * 2;                                              //0
			var EndPage = StartPage + img.ImageHeightBytes;        //2
			var StartCol = col;
			var EndCol = StartCol + img.ImageWidthPx;
			UInt32 CurrentCol;

			/* Copy the character image into the display buffer */
			for(CurrentCol = StartCol; CurrentCol < EndCol; CurrentCol++)
			{
				UInt32 CurrentPage = 0;
				for(CurrentPage = StartPage; CurrentPage < EndPage; CurrentPage++)
				{
					_displayBuffer[CurrentCol, CurrentPage] = img.ImageData[CharDataIndex];
					CharDataIndex++;
				}
			}

			/* Return the number of horizontal pixels used by the character */
			return CurrentCol - StartCol;
		}

		/// <summary>
		/// Sets all pixels in the screen buffer to 0
		/// </summary>
		public void Clear()
		{
			Array.Clear(_displayBuffer, 0, _displayBuffer.Length);
		}
		public void TurnLightOn()
		{
			SendCommand(Command.On);
		}
		public void TurnLightOff()
		{
			SendCommand(Command.Off);
		}



		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Initialize GPIO, I2C, and the display 
		/// The device may not respond to multiple Init calls without being power cycled
		/// so we allow an optional boolean to excuse failures which is useful while debugging
		/// without power cycling the display
		/// </summary>
		private async void InitI2cDevice(Boolean proceedOnFail)
		{
			var settings = new I2cConnectionSettings(_address)
			{
				BusSpeed = I2cBusSpeed.FastMode
			};
			var aqs = I2cDevice.GetDeviceSelector(_i2cControllerName);
			var dis = await DeviceInformation.FindAllAsync(aqs);

			_displayI2c = await I2cDevice.FromIdAsync(dis[0].Id, settings);
			if(_displayI2c == null && !proceedOnFail)
				throw new DeviceNotFoundException("No one I2C controllers was found!");
		}
		private void InitDisplay(Boolean proceedOnFail)
		{
			try
			{
				SendCommand(Command.ChargePumpOn);      // Turn on the internal charge pump to provide power to the screen
				SendCommand(Command.MemAddrMode);       // Set the addressing mode to "horizontal"
				SendCommand(Command.SegRemap);          // Flip the display horizontally, so it's easier to read on the breadboard
				SendCommand(Command.ComScanDir);        // Flip the display vertically, so it's easier to read on the breadboard
				SendCommand(Command.On);                // Turn the display on
			}
			catch(Exception ex)
			{
				if(!proceedOnFail)
				{
					throw new DeviceException("Display Initialization Failed", ex);
				}
			}
		}
		private void SendData(Byte[] Data)
		{
			var commandBuffer = new Byte[Data.Length + 1];
			Data.CopyTo(commandBuffer, 1);
			commandBuffer[0] = 0x40; // display buffer register

			_displayI2c.Write(commandBuffer);
		}
		private void SendCommand(Byte[] Command)
		{
			var commandBuffer = new Byte[Command.Length + 1];
			Command.CopyTo(commandBuffer, 1);
			commandBuffer[0] = 0x00; // control register

			_displayI2c.Write(commandBuffer);
		}
		private Byte[] Serialize(Byte[,] displayBuffer)
		{
			var serializedBuffer = new Byte[_screenWidthPx * _screenHeightPixels];
			var index = 0;

			// We convert our 2-dimensional array into a serialized string of bytes that will be sent out to the display
			for(var pageY = 0; pageY < _screenHeightPixels; pageY++)
			{
				for(var pixelX = 0; pixelX < _screenWidthPx; pixelX++)
				{
					serializedBuffer[index] = _displayBuffer[pixelX, pageY];
					index++;
				}
			}

			return serializedBuffer;
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		public void Dispose()
		{
			_displayI2c?.Dispose();
		}
	}
}
