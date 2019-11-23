using System;
using System.IO.Ports;
using System.Threading;

namespace ComListener
{
	class Program
    {
        static void Main(String[] args)
        {
            var avaliblePorts = SerialPort.GetPortNames();
            using (var port = new SerialPort(avaliblePorts[0], 9600))
            {
                port.DataReceived += PortOnDataReceived;
                while (true)
                {
                    if (!port.IsOpen)
                        TryToReconnect(port);

                    Thread.Sleep(1000);
                }
            }
        }
        private static void PortOnDataReceived(Object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            var serialPort = (SerialPort)sender;
            var message = serialPort.ReadLine();
            Console.WriteLine($"{serialPort.PortName}: {message}");
        }


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
        private static void TryToReconnect(SerialPort port)
        {
            try
            {
                port.Open();
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Waiting for Arduino");
            }
        }
    }
}