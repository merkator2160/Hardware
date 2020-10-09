using Core.Sandbox.Units.SbusSoftwareDecoder.Helpers;
using Core.Sandbox.Units.SbusSoftwareDecoder.Sbus;
using Core.Sandbox.Units.SbusSoftwareDecoder.Sbus.Models;
using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Core.Sandbox.Units.SbusSoftwareDecoder
{
	internal static class SbusSoftwareDecoderUnite
	{
		private const Byte _portNumber = 3;

		private static String _bufferLogFilePath;
		private static String _channelValuesFilePath;
		private static Int32 _messageCount;

		private static SbusConverter _converter;


		public static void Run()
		{
			_bufferLogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SbusStream.txt");
			if(File.Exists(_bufferLogFilePath))
				File.Delete(_bufferLogFilePath);

			_channelValuesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ChannelValues.txt");
			if(File.Exists(_channelValuesFilePath))
				File.Delete(_channelValuesFilePath);

			_converter = new SbusConverter
			{
				IsFilterEnable = false,
				IgnoreLostFrame = false
			};

			OpenPort();
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private static void PortOnDataReceived(Object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
		{
			var serialPort = (SerialPort)sender;

			if(!_converter.TryReadMessage(serialPort, out var messageBuffer))
				return;

			($"{BitConverter.ToString(messageBuffer).Replace("-", " ")} ").SaveOnDisk(_bufferLogFilePath, Encoding.UTF8, FileMode.Append);
			//($"{String.Join(" ", messageBuffer)} ").SaveOnDisk(_bufferLogFilePath, Encoding.UTF8, FileMode.Append);

			_messageCount++;

			var message = _converter.Parse(messageBuffer);
			//($"{message.ServoChannels[2]}\r\n").SaveOnDisk(_channelValuesFilePath, Encoding.UTF8, FileMode.Append);

			if(_messageCount % 10 != 0)
				return;

			Console.Clear();
			PrintResult(message);
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private static void OpenPort()
		{
			var serialPorts = SerialPort.GetPortNames();
			var device = serialPorts.First(p => p.Equals($"COM{_portNumber}"));
			using(var port = new SerialPort(device, 100000, Parity.None, 8, StopBits.One)
			{
				Handshake = Handshake.None
			})
			{
				port.DataReceived += PortOnDataReceived;
				while(true)
				{
					if(!port.IsOpen)
						OpenPort(port);

					Thread.Sleep(1000);
				}
			}
		}
		private static void OpenPort(SerialPort port)
		{
			try
			{
				port.Open();
			}
			catch(Exception)
			{
				Console.Clear();
				Console.WriteLine("Waiting for the other side");
			}
		}
		private static void PrintResult(SBusMessage message)
		{
			for(var i = 0; i < message.ServoChannels.Length; i++)
			{
				Console.WriteLine($"Channel {i + 1}: {message.ServoChannels[i]}");
			}

			Console.WriteLine();

			for(var i = 0; i < message.DigitalChannels.Length; i++)
			{
				Console.WriteLine($"Digital channel {i + 1}: {message.DigitalChannels[i]}");
			}

			Console.WriteLine();

			Console.WriteLine($"FailSafe: {message.FailSafe}");
			Console.WriteLine($"FrameLost: {message.IsFrameLost}");
		}
	}
}