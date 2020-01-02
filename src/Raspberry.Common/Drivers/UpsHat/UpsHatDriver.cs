﻿using Common.Drivers.UpsHat.Interfaces;
using System;
using System.Buffers.Binary;
using System.Device.I2c;

namespace Common.Drivers.UpsHat
{
	public sealed class UpsHatDriver : IUpsHat, IDisposable
	{
		public const Int32 DefaultAddress = 0x36;
		private readonly I2cDevice _device;


		public UpsHatDriver() : this(DefaultAddress)
		{

		}
		public UpsHatDriver(Int32 address, Int32 busId = 1)
		{
			_device = I2cDevice.Create(new I2cConnectionSettings(busId, address));
		}


		// IUpsHat ////////////////////////////////////////////////////////////////////////////////
		public Single ReadVoltage()
		{
			var inputBuffer = new Byte[2];

			_device.WriteRead(new Byte[] { 2 }, inputBuffer);
			var rawValue = BinaryPrimitives.ReadUInt16BigEndian(inputBuffer);

			return (Single)Math.Round(rawValue * 78.125 / 1000000, 2);
		}
		public Byte ReadCapacity()
		{
			var inputBuffer = new Byte[2];

			_device.WriteRead(new Byte[] { 4 }, inputBuffer);
			var rawValue = BinaryPrimitives.ReadUInt16BigEndian(inputBuffer);

			return (Byte)(rawValue / 256);
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		public void Dispose()
		{
			_device?.Dispose();
		}
	}
}