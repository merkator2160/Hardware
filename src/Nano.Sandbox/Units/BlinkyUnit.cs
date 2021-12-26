using System.Device.Gpio;
using System.Threading;

namespace Nano.Sandbox.Units
{
	internal static class BlinkyUnit
	{
		private static GpioController _gpioController;


		public static void Run()
		{
			_gpioController = new GpioController();

			var led = _gpioController.OpenPin(14, PinMode.Output);
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