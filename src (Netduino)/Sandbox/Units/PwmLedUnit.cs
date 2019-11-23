using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace Sandbox.Units
{
    public static class PwmLedUnit
    {
        public static void Run()
        {
            var dutyCyclePwm = new PWM(PWMChannels.PWM_PIN_D5, 1000, .01, false);
            dutyCyclePwm.Start();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}