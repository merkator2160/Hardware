using System;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Ks0218HatUnit
	{
		private const Double _stepSize = 0.1;
		private const Int32 _stepDelay = 500;
		private const Int32 _pwmFrequency = 50;

		private const Int32 _irPin = 16;

		private const Int32 _motor1DirectionPin = 5;
		private const Int32 _motor1SpeedPin = 6;

		private const Int32 _motor2DirectionPin = 12;
		private const Int32 _motor2SpeedPin = 13;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			//IrTest();
			MotorTest();
		}
		private void IrDiodBlinkyTest()
		{
			var ledPin = GpioController.GetDefault().OpenPin(_irPin);
			ledPin.SetDriveMode(GpioPinDriveMode.Output);

			while(true)
			{
				ledPin.Write(GpioPinValue.High);
				Thread.Sleep(1000);

				ledPin.Write(GpioPinValue.Low);
				Thread.Sleep(1000);
			}
		}
		private void MotorTest()
		{
			// 12 - L1, L3
			// 6 - L3

			// 5 - L2, L4
			// 13

			var controller = GpioController.GetDefault();

			var motor1Direction = controller.OpenPin(5);
			var motor1Enable = controller.OpenPin(18);

			motor1Direction.SetDriveMode(GpioPinDriveMode.Output);
			motor1Enable.SetDriveMode(GpioPinDriveMode.Output);

			while(true)
			{
				motor1Direction.Write(GpioPinValue.Low);
				for(var i = 0; i < 3; i++)
				{
					motor1Enable.Write(GpioPinValue.Low);
					Thread.Sleep(1000);

					motor1Enable.Write(GpioPinValue.High);
					Thread.Sleep(1000);
				}

				motor1Direction.Write(GpioPinValue.High);
				for(var i = 0; i < 3; i++)
				{
					motor1Enable.Write(GpioPinValue.Low);
					Thread.Sleep(1000);

					motor1Enable.Write(GpioPinValue.High);
					Thread.Sleep(1000);
				}
			}
		}
	}
}