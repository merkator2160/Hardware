using Microsoft.SPOT;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ToyCar
{
    public class Program
    {
        private static readonly SerialPort _transmitter = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        private static readonly SerialPort _receiver = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
        private static readonly SerialPort _sbusReceiver = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);

        private static Timer _timer;

        private static Int32 _count = 0;


        public static void Main()
        {
            _timer = new Timer(TimerTick, null, 0, 2000);

            _transmitter.Open();
            _receiver.Open();

            //_receiver.DataReceived += ReceiverOnDataReceivedEventHandler;
            _receiver.DataReceived += SerialDataReceived;
            _sbusReceiver.DataReceived += SbusReceiverOnDataReceived;

            Thread.Sleep(Timeout.Infinite);
        }

        


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
        private static void TimerTick(Object state)
        {
            if (_transmitter.IsOpen)
            {
                _count++;

                var messageToSend = _count.ToString();
                Debug.Print("Sending message: " + messageToSend);
                messageToSend += "\n";

                _transmitter.Write(Encoding.UTF8.GetBytes(messageToSend), 0, messageToSend.Length);
            }
        }
        private static void ReceiverOnDataReceivedEventHandler(Object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            if (serialPort.BytesToRead > 0)
            {
                using (var reader = new StreamReader(serialPort.BaseStream))
                {
                    Debug.Print("Message received: " + reader.ReadToEnd());
                }
            }
        }
        private static void SerialDataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            if ((e.EventType == SerialData.Chars) && (sender == _receiver))
            {
                String messageBeingReceived = String.Empty;
                const Int32 bufferSize = 1024;
                var buffer = new Byte[bufferSize];

                var amount = ((SerialPort)sender).Read(buffer, 0, bufferSize);
                if (amount > 0)
                {
                    var characters = Encoding.UTF8.GetChars(buffer);
                    for (var index = 0; index < amount; index++)
                    {
                        if (buffer[index] == '\n')
                        {
                            Debug.Print("Message received: " + messageBeingReceived);
                            messageBeingReceived = "";
                        }
                        else
                        {
                            messageBeingReceived += characters[index];
                        }
                    }
                }
            }
        }
        private static void SbusReceiverOnDataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            if (serialPort.BytesToRead > 0)
            {
                using (var reader = new StreamReader(serialPort.BaseStream))
                {
                    Debug.Print("SBUS received: " + reader.ReadToEnd());
                }
            }
        }


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
    }
}