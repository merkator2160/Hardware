using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System;
using System.Threading;

namespace Sandbox.Units
{
    internal static class FlashingLedUnit
    {
        public static void Run()
        {
            // configure an output port for us to "write" to the LED
            var led = new OutputPort(Pins.ONBOARD_LED, false);

            // note that if we didn't have the SecretLabs.NETMF.Hardware.Netduino DLL, we could also manually access it this way:
            //OutputPort led = new OutputPort(Cpu.Pin.GPIO_Pin10, false); 
            Int32 i = 0;
            while (true)
            {
                led.Write(true);
                Thread.Sleep(250);
                led.Write(false);
                Thread.Sleep(250);

                //Debug.Print("Looping " + i);
                i++;
            }
        }
    }
}