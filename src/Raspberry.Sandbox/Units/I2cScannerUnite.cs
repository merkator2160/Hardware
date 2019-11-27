using Common.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Raspberry.Sandbox.Units
{
	internal sealed class I2cScannerUnite
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			// Inbox driver required
			var devices = FindDevicesAsync().GetAwaiter().GetResult();
		}
		public static async Task<Byte[]> FindDevicesAsync()
		{
			var returnValue = new List<Byte>();
			var deviceSelector = I2cDevice.GetDeviceSelector();

			var dis = await DeviceInformation.FindAllAsync(deviceSelector);
			if(dis.Count <= 0)
				throw new DeviceNotFoundException("No one I2C controllers was found!");

			const Int32 minimumAddress = 0x08;
			const Int32 maximumAddress = 0x77;

			for(Byte address = minimumAddress; address <= maximumAddress; address++)
			{
				var settings = new I2cConnectionSettings(address)
				{
					BusSpeed = I2cBusSpeed.FastMode,
					SharingMode = I2cSharingMode.Shared
				};

				using(var device = await I2cDevice.FromIdAsync(dis[0].Id, settings))
				{
					if(device == null)
						continue;

					try
					{
						var writeBuffer = new Byte[1] { 0 };
						device.Write(writeBuffer);
						returnValue.Add(address);
					}
					catch { }   // If the address is invalid, an exception will be thrown
				}
			}

			return returnValue.ToArray();
		}
	}
}