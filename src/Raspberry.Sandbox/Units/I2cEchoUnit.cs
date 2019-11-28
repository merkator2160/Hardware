using Common.Models.Exceptions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Raspberry.Sandbox.Units
{
	internal sealed class I2cEchoUnit
	{
		private const Int32 _arduinoAddress = 0x08;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			RunAsync(taskInstance).GetAwaiter().GetResult();
		}
		public async Task RunAsync(IBackgroundTaskInstance taskInstance)
		{
			using(var pwmDevice = await GetDeviceAsync())
			{
				var outputBuffer = new Byte[] { 10, 11, 12 };
				var inputBuffer = new Byte[outputBuffer.Length];

				pwmDevice.Write(outputBuffer);
				//pwmDevice.WriteRead(outputBuffer, inputBuffer);
				pwmDevice.Read(inputBuffer);

				foreach(var x in inputBuffer)
				{
					Debug.WriteLine(x);
				}
			}
		}

		// SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
		private static async Task<I2cDevice> GetDeviceAsync()
		{
			var dis = await DeviceInformation.FindAllAsync(I2cDevice.GetDeviceSelector("I2C1"));
			if(dis.Count <= 0)
				throw new DeviceNotFoundException("No one I2C controllers was found!");

			var device = await I2cDevice.FromIdAsync(dis[0].Id, new I2cConnectionSettings(_arduinoAddress)
			{
				BusSpeed = I2cBusSpeed.FastMode,
				SharingMode = I2cSharingMode.Exclusive
			});
			if(device == null)
				throw new DeviceNotFoundException("PWM reader was not found!");

			return device;
		}
	}
}