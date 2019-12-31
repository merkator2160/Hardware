using Common.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.I2c;

namespace Raspberry.Sandbox.Units
{
	internal sealed class UpsHatUnit
	{
		private const Int32 _upsHatAddress = 54;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			RunAsync(taskInstance).GetAwaiter().GetResult();
		}
		public async Task RunAsync(IBackgroundTaskInstance taskInstance)
		{
			using(var i2cDevice = await I2cScanner.GetDeviceAsync(_upsHatAddress))
			{
				var inputBuffer = new Byte[2];

				i2cDevice.Read(inputBuffer);

				foreach(var x in inputBuffer)
				{
					Debug.WriteLine(x);
				}
			}
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private void ReadVoltage(I2cDevice device)
		{

		}
		private void ReadCapacity(I2cDevice device)
		{

		}
	}
}