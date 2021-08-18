using System.Device.Gpio;
using System.Threading;

namespace Nano.Sandbox.Units
{
	public static class BlinkyUnit
	{
		private static GpioController gpioController;


		public static void Run()
		{
			gpioController = new GpioController();

			var led = gpioController.OpenPin(14, PinMode.Output);
			led.Write(PinValue.Low);

			while(true)
			{
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(525);
			}
		}
	}
}