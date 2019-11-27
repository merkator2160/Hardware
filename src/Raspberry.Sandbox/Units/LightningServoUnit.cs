using Common.Const;
using Common.Models.Exceptions;
using Microsoft.IoT.Lightning.Providers;
using System;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Devices.Pwm;

namespace Raspberry.Sandbox.Units
{
	internal sealed class LightningServoUnit
	{
		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			var pwmControllers = PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider()).GetAwaiter().GetResult();
			if(pwmControllers == null)
				throw new ControllerNotFoundException("There is no available controllers on the board!");

			var pwmController = pwmControllers[1];
			pwmController.SetDesiredFrequency(50);

			var servoGpioPin = pwmController.OpenPin(5);

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