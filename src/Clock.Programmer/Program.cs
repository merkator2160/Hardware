using Clock.Programmer.Models.Enums;
using System;
using System.IO.Ports;
using System.Threading;

namespace Clock.Programmer
{
    class Program
    {
        static void Main(String[] args)
        {
            var avaliblePorts = SerialPort.GetPortNames();
            using (var port = new SerialPort(avaliblePorts[0], 9600))
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