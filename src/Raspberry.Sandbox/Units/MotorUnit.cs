using Common.Const;
using Microsoft.IoT.Lightning.Providers;
using System;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Devices.Pwm;

namespace Raspberry.Sandbox.Units
{
	public sealed class MotorUnit
	{
		private const Int32 _motorPinNumber1 = 20;
		private const Int32 _motorPinNumber2 = 21;

		private readonly PwmPin motorPin1;
		private readonly PwmPin motorPin2;


		public MotorUnit()
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
			const Double _stepSize = 0.1;
			const Int32 _stepDelay = 500;

			motorPin1.Start();
			motorPin2.Start();

			while(true)
			{
				// MoveForward
				var speed = Hw354.MinSpeed;
				while(speed < Hw354.MaxSpeed)
				{
					MoveForward(speed);
					Thread.Sleep(_stepDelay);
					speed += _stepSize;
				}

				speed = Hw354.MaxSpeed;
				while(speed > Hw354.MinSpeed)
				{
					MoveForward(speed);
					Thread.Sleep(_stepDelay);
					speed -= _stepSize;
				}

				// MoveBackward
				speed = Hw354.MinSpeed;
				while(speed < Hw354.MaxSpeed)
				{
					MoveBackward(speed);
					Thread.Sleep(_stepDelay);
					speed += _stepSize;
				}

				speed = Hw354.MaxSpeed;
				while(speed > Hw354.MinSpeed)
				{
					MoveBackward(speed);
					Thread.Sleep(_stepDelay);
					speed -= _stepSize;
				}
			}
		}
		private void MoveForward(Double speed)
		{
			motorPin1.SetActiveDutyCyclePercentage(speed);
			motorPin2.SetActiveDutyCyclePercentage(0);
		}
		private void MoveBackward(Double speed)
		{
			motorPin1.SetActiveDutyCyclePercentage(0);
			motorPin2.SetActiveDutyCyclePercentage(speed);
		}
		private void Standby()
		{
			motorPin1.SetActiveDutyCyclePercentage(0);
			motorPin2.SetActiveDutyCyclePercentage(0);
		}
		private void Break()
		{
			motorPin1.SetActiveDutyCyclePercentage(1);
			motorPin2.SetActiveDutyCyclePercentage(1);
		}
	}
}