using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Core.Sandbox.Units.ArduinoListener
{
	internal class SerialValueListenerUnit
	{
		private const Byte _portNumber = 5;
		private const Int32 _portSpeed = 9600;
		private const Boolean _refreshAlways = false;

		private static Dictionary<String, String> _values;

		public static void Run()
		{
			_values = new Dictionary<String, String>();

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

			ParseValues(log);
			PrintValues();

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
		private static void ParseValues(String str)
		{
			var clearStr = str.Trim('\\', 'n', 'r');
			var variables = clearStr.Split(',');

			if(_refreshAlways)
				_values.Clear();

			foreach(var x in variables)
			{
				var valuePair = x.Split(':');

				if(!_values.ContainsKey(valuePair[0]))
				{
					_values.Add(valuePair[0], valuePair[1]);
					return;
				}

				_values[valuePair[0]] = valuePair[1];
			}
		}
		private static void PrintValues()
		{
			Console.Clear();

			foreach(var (key, value) in _values)
			{
				Console.WriteLine($"{key}: {value}");
			}
		}
	}
}