using Microsoft.IoT.Lightning.Providers;
using System;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Devices.Pwm;

namespace Raspberry.Sandbox.Units
{
	public sealed class LepinServoUnit
	{
		private const Int32 _motorPinNumber1 = 20;
		private const Int32 _motorPinNumber2 = 21;

		private readonly PwmPin motorPin1;
		private readonly PwmPin motorPin2;


		public LepinServoUnit()
		{
			var pwmControllers = PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider()).GetAwaiter().GetResult();
			var pwmController = pwmControllers[1];
			pwmController.SetDesiredFrequency(50);

			motorPin1 = pwmController.OpenPin(_motorPinNumber1);
			motorPin2 = pwmController.OpenPin(_motorPinNumber2);
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			const Int32 _delay = 500;

			motorPin1.Start();
			motorPin2.Start();

			while(true)
			{
				TurnRight();
				Thread.Sleep(_delay);

				Return();
				Thread.Sleep(_delay);

				TurnLeft();
				Thread.Sleep(_delay);

				Return();
				Thread.Sleep(_delay);
			}
		}
		private void TurnRight()
		{
			motorPin1.SetActiveDutyCyclePercentage(1);
			motorPin2.SetActiveDutyCyclePercentage(0);
		}
		private void TurnLeft()
		{
			motorPin1.SetActiveDutyCyclePercentage(0);
			motorPin2.SetActiveDutyCyclePercentage(1);
		}
		private void Return()
		{
			motorPin1.SetActiveDutyCyclePercentage(0);
			motorPin2.SetActiveDutyCyclePercentage(0);
		}
	}
}