using Common.Models.Exceptions;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Raspberry.Sandbox.Units
{
	internal sealed class I2cListenerUnit
	{
		private const Int32 _arduinoAddress = 0x08;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			RunAsync(taskInstance).GetAwaiter().GetResult();
		}
		public async Task RunAsync(IBackgroundTaskInstance taskInstance)
		{
			using(var pwmDevice = await GetPwmDeviceAsync())
			{
				//pwmReader.Write(new Byte[] { 10, 11, 12 });

				while(true)
				{
					var values = ReadValues(pwmDevice);
					var str = JsonConvert.SerializeObject(values);

					Debug.WriteLine(str);
				}
			}
		}

		// SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
		private static async Task<I2cDevice> GetPwmDeviceAsync()
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
		private static Int16[] ReadValues(I2cDevice pwmDevice)
		{
			var buffer = new Byte[18];

			pwmDevice.Read(buffer);
			return ConvertBufferInt16(buffer);
		}
		private static Int16[] ConvertBufferInt16(Byte[] buffer)
		{
			const Byte intSize = 2;
			var values = new Int16[buffer.Length / intSize];

			for(var i = 0; i < values.Length; i++)
			{
				var currentInt = buffer.Skip(i * intSize).Take(intSize).ToArray();
				values[i] = BitConverter.ToInt16(currentInt, 0);
			}

			return values;
		}
	}
}