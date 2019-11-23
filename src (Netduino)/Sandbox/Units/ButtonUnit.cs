using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Sandbox.Units
{
    internal static class ButtonUnit
    {
        static readonly OutputPort _led = new OutputPort(Pins.ONBOARD_LED, false);
        static readonly InputPort _button = new InputPort(Pins.ONBOARD_BTN, false, Port.ResistorMode.Disabled);


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public static void Run()
        {
            // turn the LED off initially
            _led.Write(false);

            // run forever
            while (true)
            {
                // set the onboard LED output to be the input of the button
                _led.Write(_button.Read());
            }
        }
    }
}