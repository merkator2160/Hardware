using Microsoft.SPOT;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Sandbox.Units
{
    public static class SerialPortUnit
    {
        private static readonly SerialPort _transmitter = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        private static readonly SerialPort _receiver = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);

        private static Timer _timer = new Timer(Timer_Interrupt, null, 0, 2000);

        private static Int32 _count = 0;
        private static String _messageBeingReceived = "";


        public static void Run()
        {
            _transmitter.Open();
            _receiver.Open();
            _receiver.DataReceived += SerialDataReceived;
            Thread.Sleep(Timeout.Infinite);
        }


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
        private static void SerialDataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            if ((e.EventType == SerialData.Chars) && (sender == _receiver))
            {
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
                            Debug.Print("Message received: " + _messageBeingReceived);
                            _messageBeingReceived = "";
                        }
                        else
                        {
                            _messageBeingReceived += characters[index];
                        }
                    }
                }
            }
        }
        private static void SerialDataReceived2(Object sender, SerialDataReceivedEventArgs e)
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
        private static void Timer_Interrupt(Object state)
        {
            if (_transmitter.IsOpen)
            {
                _count++;
                var messageToSend = "Message №" + _count;
                Debug.Print("Sending message: " + messageToSend);
                messageToSend += "\n";
                _transmitter.Write(Encoding.UTF8.GetBytes(messageToSend), 0, messageToSend.Length);
            }
        }
    }
}