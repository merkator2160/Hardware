using Core.Sandbox.Units.ClockProgrammer.Models.Enums;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Core.Sandbox.Units.ClockProgrammer
{
	internal static class ClockProgrammerUnit
	{
		private const Byte _portNumber = 8;
		private const Int32 _portSpeed = 9600;


		public static void Run()
		{
			var device = SerialPort.GetPortNames().First(p => p.Equals($"COM{_portNumber}"));
			using(var port = new SerialPort(device, _portSpeed))
			{
				port.DataReceived += PortOnDataReceived;
				port.Open();

				SetTime(port);

				Thread.Sleep(2000);

				GetTime(port);

				Console.ReadKey();
			}
		}
		private static void SetTime(SerialPort port)
		{
			var currentTime = DateTime.Now;
			var commandStr = $"{(Byte)Command.SetTime}:{currentTime.Year}:{currentTime.Month}:{currentTime.Day}:{currentTime.Hour}:{currentTime.Minute}:{currentTime.Second}:{(Byte)currentTime.DayOfWeek + 1}";
			port.WriteLine(commandStr);
		}
		private static void GetTime(SerialPort port)
		{
			port.WriteLine($"{(Byte)Command.GetTime}");
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private static void PortOnDataReceived(Object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
		{
			var serialPort = (SerialPort)sender;
			var message = serialPort.ReadLine();
			Console.WriteLine($"{serialPort.PortName}: {message}");
		}
	}
}