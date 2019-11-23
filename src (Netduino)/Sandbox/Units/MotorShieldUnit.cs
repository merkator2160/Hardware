using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace Sandbox.Units
{
    public static class MotorShieldUnit
    {
        public static void Run()
        {
            var motorPwm1 = new PWM(PWMChannels.PWM_PIN_D10, 100, .2, false);
            var direction1 = new OutputPort(Pins.GPIO_PIN_D9, true);

            motorPwm1.Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}