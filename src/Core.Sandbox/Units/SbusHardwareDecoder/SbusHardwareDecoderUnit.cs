using Core.Sandbox.Units.SbusHardwareDecoder.Models;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Core.Sandbox.Units.SbusHardwareDecoder
{
	internal static class SbusHardwareDecoderUnit
	{
		private const Byte _portNumber = 9;
		private const Int32 _portSpeed = 115200;

		public static void Run()
		{
			var serialPorts = SerialPort.GetPortNames();
			var device = serialPorts.First(p => p.Equals($"COM{_portNumber}"));
			using(var port = new SerialPort(device, _portSpeed)
			{
				Handshake = Handshake.None
			})
			{
				port.DataReceived += PortOnDataReceived;
				while(true)
				{
					if(!port.IsOpen)
						TryReconnect(port);

					Thread.Sleep(1000);
				}
			}
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private static void PortOnDataReceived(Object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
		{
			var serialPort = (SerialPort)sender;

			var message = serialPort.ReadLine();

			var processedMessage = ParseMessage(message);

			Console.Clear();
			PrintResult(processedMessage);
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private static void TryReconnect(SerialPort port)
		{
			try
			{
				port.Open();
			}
			catch(Exception)
			{
				Console.Clear();
				Console.WriteLine("Waiting for SBUS decoder");
			}
		}
		private static void PrintResult(SBusMessage message)
		{
			for(var i = 0; i < message.ServoChannels.Length; i++)
			{
				Console.WriteLine($"Channel {i + 1}: {message.ServoChannels[i]}");
			}

			Console.WriteLine();

			Console.WriteLine($"FailSafe: {message.FailSafe}");
			Console.WriteLine($"FrameLost: {message.FrameLost}");
		}
		private static SBusMessage ParseMessage(String messageStr)
		{
			var messageComponents = messageStr
				.Split(new[] { " ", "\r" }, StringSplitOptions.RemoveEmptyEntries)
				.Select(x =>
				{
					var headerAndValue = x.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
					return new MessageComponent()
					{
						Header = headerAndValue[0],
						Value = headerAndValue[1]
					};
				}).ToArray();

			return new SBusMessage()
			{
				ServoChannels = messageComponents.Take(18).Select(p => ReadChannelValue(p.Value)).ToArray(),
				FailSafe = messageComponents[18].Value,
				FrameLost = messageComponents[19].Value
			};
		}
		private static UInt16 ReadChannelValue(String value)
		{
			if(Single.TryParse(value, out var result))
				return (UInt16)result;

			return 0;
		}
	}
}