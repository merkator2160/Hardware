using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Core.Sandbox.Units
{
	internal static class ComListenerUnit
	{
		private const Byte _portNumber = 9;
		private const Int32 _portSpeed = 115200;


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

			Console.WriteLine(serialPort.ReadLine());
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
	}
}