using System;
using System.Device.I2c;

namespace Common.Drivers.Bmxx80
{
	/// <summary>
	/// Represents a BME280 temperature and barometric pressure sensor.
	/// </summary>
	public class Bmp280Driver : Bmx280Base
	{
		/// <summary>
		/// The expected chip ID of the BMP280.
		/// </summary>
		private const Byte DeviceId = 0x58;

		/// <summary>
		/// Initializes a new instance of the <see cref="Bmp280Driver"/> class.
		/// </summary>
		/// <param name="i2cDevice">The <see cref="I2cDevice"/> to create with.</param>
		public Bmp280Driver(I2cDevice i2cDevice) : base(DeviceId, i2cDevice)
		{
			_communicationProtocol = CommunicationProtocol.I2c;
		}
	}
}
