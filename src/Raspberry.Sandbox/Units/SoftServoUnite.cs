using Common.Const;
using Microsoft.IoT.DeviceCore.Pwm;
using Microsoft.IoT.Devices.Pwm;
using System;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class SoftServoUnite
	{
		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var pwmManager = new PwmProviderManager())
			{
				pwmManager.Providers.Add(new SoftPwm());

				var pwmControllers = pwmManager.GetControllersAsync().GetAwaiter().GetResult();
				if(pwmControllers == null)
					return;

				var pwmController = pwmControllers.First();
				var servoGpioPin = pwmController.OpenPin(5);

				pwmController.SetDesiredFrequency(50);

				const Double _stepSize = 0.001;
				const Int32 _stepDelay = 10;

				var position = Sg90.MinPosition;
				servoGpioPin.SetActiveDutyCyclePercentage(position);
				servoGpioPin.Start();

				while(true)
				{
					while(position < Sg90.MaxPosition)
					{
						servoGpioPin.SetActiveDutyCyclePercentage(position);
						Thread.Sleep(_stepDelay);
						position = position + _stepSize;
					}
					Thread.Sleep(1000);

					while(position > Sg90.MinPosition)
					{
						servoGpioPin.SetActiveDutyCyclePercentage(position);
						Thread.Sleep(_stepDelay);
						position = position - _stepSize;
					}
					Thread.Sleep(1000);
				}

				servoGpioPin.Stop();
			}
		}
	}
}