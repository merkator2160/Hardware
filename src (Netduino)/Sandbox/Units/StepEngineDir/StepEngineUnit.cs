using Sandbox.Units.StepEngineDir.Enums;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace Sandbox.Units.StepEngineDir
{
    internal static class StepEngineUnit
    {
        public static void Run()
        {
            using (var stepEngine = new Stepper(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, Pins.GPIO_PIN_D2, Pins.GPIO_PIN_D3))
            {
                while (true)
                {
                    stepEngine.Move(Direction.Forward, Stepper.StepsPerRevolutionSinglePrecisione, 2);
                    Thread.Sleep(2000);
                    stepEngine.Move(Direction.Backward, Stepper.StepsPerRevolutionSinglePrecisione, 2);
                    Thread.Sleep(2000);
                }
            }
        }
    }
}