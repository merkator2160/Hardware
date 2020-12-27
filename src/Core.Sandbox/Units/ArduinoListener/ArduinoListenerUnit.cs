using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Core.Sandbox.Units.ArduinoListener
{
	internal class ArduinoListenerUnit
	{
		private const Byte _portNumber = 5;
		private const Int32 _portSpeed = 9600;


		public static void Run()
		{


			var serialPorts = SerialPort.GetPortNames();
			var device = serialPorts.First(p => p.Equals($"COM{_portNumber}"));
			using(var port = new SerialPort(device, _portSpeed))
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
			var log = serialPort.ReadLine();

			ParseLog(log);

			Console.WriteLine();
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
				Console.WriteLine("Waiting for connection!");
			}
		}
		private static void ParseLog(String str)
		{
			var clearStr = str.Trim('\\', 'n', 'r');
			var variables = clearStr.Split(',');

			Console.Clear();
			foreach(var x in variables)
			{
				var valuePair = x.Split(':');

				Console.WriteLine($"{valuePair[0]}: {valuePair[1]}");
			}
		}
	}
}