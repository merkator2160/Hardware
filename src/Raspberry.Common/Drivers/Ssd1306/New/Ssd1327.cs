using Common.Drivers.Ssd1306.New.Commands;
using Common.Drivers.Ssd1306.New.Commands.Ssd1327Commands;
using System;
using System.Device.I2c;

namespace Common.Drivers.Ssd1306.New
{
	/// <summary>
	/// Represents SSD1327 OLED display
	/// </summary>
	public class Ssd1327 : Ssd13xx
	{
		private const Byte Command_Mode = 0x80;
		private const Byte Data_Mode = 0x40;
		private const Int32 CleaningBufferSize = 48 * 96;

		/// <summary>
		/// Initializes new instance of Ssd1327 device that will communicate using I2C bus.
		/// </summary>
		/// <param name="i2cDevice">The I2C device used for communication.</param>
		public Ssd1327(I2cDevice i2cDevice)
			: base(i2cDevice)
		{
		}

		/// <summary>
		/// Sets column address
		/// </summary>
		/// <param name="startAddress">Start address</param>
		/// <param name="endAddress">End address</param>
		public void SetColumnAddress(Byte startAddress = 0x08, Byte endAddress = 0x37)
		{
			SendCommand(new SetColumnAddress(startAddress, endAddress));
		}

		/// <summary>
		/// Sets row address
		/// </summary>
		/// <param name="startAddress">Start address</param>
		/// <param name="endAddress">End address</param>
		public void SetRowAddress(Byte startAddress = 0x00, Byte endAddress = 0x5f)
		{
			SendCommand(new SetRowAddress(startAddress, endAddress));
		}

		/// <summary>
		/// Clears the display
		/// </summary>
		public void ClearDisplay()
		{
			SendCommand(new SetDisplayOff());
			SetColumnAddress();
			SetRowAddress();
			var data = new Byte[CleaningBufferSize];
			SendData(data);
			SendCommand(new SetDisplayOn());
		}

		/// <summary>
		/// Send a command to the display controller.
		/// </summary>
		/// <param name="command">The command to send to the display controller.</param>
		public void SendCommand(Byte command)
		{
			Span<Byte> writeBuffer = new Byte[] { Command_Mode, command };

			_i2cDevice.Write(writeBuffer);
		}

		/// <summary>
		/// Sends command to the device
		/// </summary>
		/// <param name="command">Command being send</param>
		public void SendCommand(ISsd1327Command command)
		{
			SendCommand((ICommand)command);
		}

		/// <summary>
		/// Sends command to the device
		/// </summary>
		/// <param name="command">Command being send</param>
		public override void SendCommand(ISharedCommand command)
		{
			SendCommand(command);
		}

		/// <summary>
		/// Send data to the display controller.
		/// </summary>
		/// <param name="data">The data to send to the display controller.</param>
		public void SendData(Byte data)
		{
			Span<Byte> writeBuffer = new Byte[] { Data_Mode, data };

			_i2cDevice.Write(writeBuffer);
		}

		private void SendCommand(ICommand command)
		{
			var commandBytes = command.GetBytes();

			if(commandBytes == null)
			{
				throw new ArgumentNullException(nameof(commandBytes));
			}

			if(commandBytes.Length == 0)
			{
				throw new ArgumentException("The command did not contain any bytes to send.");
			}

			foreach(var item in commandBytes)
			{
				SendCommand(item);
			}
		}
	}
}
