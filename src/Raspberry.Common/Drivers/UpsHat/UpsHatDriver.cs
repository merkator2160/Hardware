using Common.Drivers.UpsHat.Interfaces;
using Common.Helpers;
using System;
using Windows.Devices.I2c;

namespace Common.Drivers.UpsHat
{
	public sealed class UpsHatDriver : IUpsHat, IDisposable
	{
		private const Byte _defaultAddress = 54;
		private readonly I2cDevice _device;


		public UpsHatDriver() : this(_defaultAddress)
		{

		}
		public UpsHatDriver(Int32 address)
		{
			_device = I2cScanner.GetDeviceAsync(address).GetAwaiter().GetResult();
		}


		// IUpsHat ////////////////////////////////////////////////////////////////////////////////
		public Single ReadVoltage()
		{
			var inputBuffer = new Byte[2];

			_device.WriteRead(new Byte[] { 2 }, inputBuffer);

			Array.Reverse(inputBuffer);
			var rawValue = BitConverter.ToUInt16(inputBuffer, 0);

			return (Single)Math.Round(rawValue * 78.125 / 1000000, 2);
		}
		public Byte ReadCapacity()
		{
			var inputBuffer = new Byte[2];

			_device.WriteRead(new Byte[] { 4 }, inputBuffer);

			Array.Reverse(inputBuffer);
			var rawValue = BitConverter.ToUInt16(inputBuffer, 0);

			return (Byte)(rawValue / 256);
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		public void Dispose()
		{
			_device?.Dispose();
		}
	}
}