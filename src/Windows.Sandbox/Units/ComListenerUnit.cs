using System;
using System.IO.Ports;
using System.Threading;

namespace Windows.Sandbox.Units
{
	public static class ComListenerUnit
	{
		public static void Run()
		{
			var availablePorts = SerialPort.GetPortNames();
			using(var port = new SerialPort(availablePorts[0], 9200))
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
			Console.WriteLine($"{serialPort.PortName}: {message}");
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