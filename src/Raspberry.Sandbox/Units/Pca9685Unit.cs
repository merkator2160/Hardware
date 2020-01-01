using Common.Const;
using Common.Drivers.Pca9685;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Pca9685Unit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			// address 64 and 112

			using(var pca9685 = new Pca9685Driver(64, 1, 50))
			{
				while(true)
				{
					pca9685.SetDutyCycleAllChannels(Sg90.MinPosition);
					Thread.Sleep(2000);

					pca9685.SetDutyCycleAllChannels(Sg90.MaxPosition);
					Thread.Sleep(2000);
				}

				//var firstChannel = pca9685.CreatePwmChannel(1);
				//var secondChannel = pca9685.CreatePwmChannel(1);

				//firstChannel.DutyCycle = 0.0;

				//Thread.Sleep(1000);

				//firstChannel.DutyCycle = 0.3;

				//Thread.Sleep(1000);

				//firstChannel.DutyCycle = 0.5;

				//Thread.Sleep(1000);

				//firstChannel.DutyCycle = 0.7;

				//Thread.Sleep(1000);

				//firstChannel.DutyCycle = 1.0;

				//secondChannel.DutyCycle = 1.0;

				// note: SetDutyCycleAllChannels cannot be used anymore
				//       because it would interfere with firstChannel and secondChannel setting
				//       this cannot be done either:
				//       pca9685.SetDutyCycle(1, 0.7);

				//pca9685.SetDutyCycle(0, 0.7);

				Debug.WriteLine("Done!");
			}
		}
	}
}