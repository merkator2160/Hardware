﻿using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Core.Sandbox.Units
{
	internal class ComWriterUnite
	{
		private static Int32 _led1 = 35;
		private static Int32 _led2 = 11;

		// Definitions for COM
		private const Byte _portNumber = 8;
		private const Int32 _portSpeed = 9600;


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
				Console.WriteLine("Waiting for SBUS decoder");
			}
		}
		private static String CreateMessage()
		{
			return $"Led1={_led1++},Led2={_led2++}";
		}
	}
}