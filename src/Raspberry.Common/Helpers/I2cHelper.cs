using Common.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Common.Helpers
{
	public static class I2cHelper
	{
		public static async Task<Byte[]> ScanBusAsync()
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
				using(var device = await I2cDevice.FromIdAsync(dis[0].Id, new I2cConnectionSettings(address)
				{
					BusSpeed = I2cBusSpeed.FastMode,
					SharingMode = I2cSharingMode.Shared
				}))
				{
					if(device == null)
						continue;

					try
					{
						device.Write(new Byte[] { 0 });
						returnValue.Add(address);
					}
					catch { }   // If the address is invalid, an exception will be thrown
				}
			}

			return returnValue.ToArray();
		}
		public static async Task<I2cDevice> GetDeviceAsync(Int32 address)
		{
			var dis = await DeviceInformation.FindAllAsync(I2cDevice.GetDeviceSelector("I2C1"));
			if(dis.Count <= 0)
				throw new DeviceNotFoundException("No one I2C controllers was found!");

			var device = await I2cDevice.FromIdAsync(dis[0].Id, new I2cConnectionSettings(address)
			{
				BusSpeed = I2cBusSpeed.FastMode,
				SharingMode = I2cSharingMode.Exclusive
			});
			if(device == null)
				throw new DeviceNotFoundException($"Device with address: {address} was not found!");

			return device;
		}
	}
}