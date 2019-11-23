using Netduino.Foundation.Servos;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace Sandbox.Units
{
    public static class ServoUnit
    {
        public static void Run()
        {
            var servo = new Servo(PWMChannels.PWM_PIN_D9);

            servo.Angle = 0;
            Thread.Sleep(2000);
            servo.Angle = 180;

            //while (true)
            //{
            //    for (int angle = 0; angle <= 180; angle++)
            //    {
            //        servo.Angle = angle;
            //        Thread.Sleep(10);
            //    }
            //    for (int angle = 179; angle > 0; angle--)
            //    {
            //        servo.Angle = angle;
            //        Thread.Sleep(10);
            //    }
            //}
        }
    }
}