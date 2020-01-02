using Common.Const;
using Common.Drivers;
using Common.Drivers.Pca9685;
using System;
using System.Collections.Generic;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Pca9685Unit
	{
		private const Double _stepSize = 0.001;
		private const Int32 _stepDelay = 10;
		private const Byte _driverSize = 16;

		// I2C addresses 64 and 112

		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var driver = new Pca9685Driver(64, 1, 50))
			{
				AllServosToCenter(driver);

				Debug.WriteLine("Done!");
			}
		}
		private static void WiggleAllChannels(Pca9685Driver driver)
		{
			var position = Sg90.MinPositionFrq;

			while(true)
			{
				while(position < Sg90.MaxPositionFrq)
				{
					driver.SetDutyCycleAllChannels(position);
					Thread.Sleep(_stepDelay);
					position += _stepSize;
				}
				Thread.Sleep(1000);

				while(position > Sg90.MinPositionFrq)
				{
					driver.SetDutyCycleAllChannels(position);
					Thread.Sleep(_stepDelay);
					position -= _stepSize;
				}
				Thread.Sleep(1000);
			}
		}
		private static void WiggleAllChannelsIndependent(Pca9685Driver driver)
		{
			var taskList = new List<Task>(_driverSize);

			for(var i = 0; i < _driverSize - 1; i++)
			{
				var channel = driver.CreatePwmChannel(i);
				taskList.Add(Task.Run(() => WiggleChannel(channel)));
			}

			Task.WaitAll(taskList.ToArray());
		}
		private static void WiggleChannel(PwmChannel channel)
		{
			var position = Sg90.MinPositionFrq;

			while(true)
			{
				while(position < Sg90.MaxPositionFrq)
				{
					channel.DutyCycle = position;
					Thread.Sleep(_stepDelay);
					position += _stepSize;
				}
				Thread.Sleep(1000);

				while(position > Sg90.MinPositionFrq)
				{
					channel.DutyCycle = position;
					Thread.Sleep(_stepDelay);
					position -= _stepSize;
				}
				Thread.Sleep(1000);
			}
		}
		private static void WiggleAllServosIndependent(Pca9685Driver driver)
		{
			var taskList = new List<Task>(_driverSize);

			for(var i = 0; i < _driverSize - 1; i++)
			{
				var servo = driver.CreateSg90Servo(i);
				taskList.Add(Task.Run(() => WiggleServos(servo)));
			}

			Task.WaitAll(taskList.ToArray());
		}
		private static void WiggleServos(ServoMotor servo)
		{
			const Int32 maxAngle = Sg90.MaxAngle;
			const Int32 minAngle = 0;

			var currentAngle = maxAngle;

			while(true)
			{
				while(currentAngle < maxAngle)
				{
					servo.WriteAngle(currentAngle);
					Thread.Sleep(_stepDelay);
					currentAngle++;
				}
				Thread.Sleep(1000);

				while(currentAngle > minAngle)
				{
					servo.WriteAngle(currentAngle);
					Thread.Sleep(_stepDelay);
					currentAngle--;
				}
				Thread.Sleep(1000);
			}
		}
		private void AllServosToCenter(Pca9685Driver driver)
		{
			for(var i = 0; i < _driverSize - 1; i++)
			{
				using(var servo = driver.CreateSg90Servo(i))
				{
					servo.WriteAngle(90);
				}
			}
		}
	}
}