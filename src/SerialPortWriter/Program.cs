﻿using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace SerialPortWriter
{
	class Program
	{
		private static Int32 _led1 = 35;
		private static Int32 _led2 = 11;


		static void Main(String[] args)
		{
			var availablePorts = SerialPort.GetPortNames();
			var chosenPort = availablePorts.First(p => p.Contains("COM3"));

			using(var port = new SerialPort(chosenPort, 9200)
			{
				Handshake = Handshake.None
			})
			{
				port.DataReceived += PortOnDataReceived;
				while(true)
				{
					if(!port.IsOpen)
						TryReconnect(port);

					var message = CreateMessage();

					port.WriteLine(message);
					Console.WriteLine($"Message was sent: {message}");

					Thread.Sleep(1000);
				}
			}
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private static void PortOnDataReceived(Object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
		{
			var serialPort = (SerialPort)sender;

			var message = serialPort.ReadLine();
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
		private static String CreateMessage()
		{
			return $"Led1={_led1++},Led2={_led2++}";
		}
	}
}
