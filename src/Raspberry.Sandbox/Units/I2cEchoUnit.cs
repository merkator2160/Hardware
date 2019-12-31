using Common.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class I2cEchoUnit
	{
		private const Int32 _arduinoAddress = 8;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			RunAsync(taskInstance).GetAwaiter().GetResult();
		}
		public async Task RunAsync(IBackgroundTaskInstance taskInstance)
		{
			using(var i2cDevice = await I2cScanner.GetDeviceAsync(_arduinoAddress))
			{
				var outputBuffer = new Byte[] { 10, 11, 12 };
				var inputBuffer = new Byte[outputBuffer.Length];

				i2cDevice.Write(outputBuffer);
				//i2cDevice.WriteRead(outputBuffer, inputBuffer);
				i2cDevice.Read(inputBuffer);

				foreach(var x in inputBuffer)
				{
					Debug.WriteLine(x);
				}
			}
		}
	}
}