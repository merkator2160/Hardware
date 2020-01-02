using Common.Helpers;
using System;
using System.Device.I2c;

namespace Common.Drivers.Rtc.Pcf8563
{
	/// <summary>
	/// Realtime Clock PCF8563
	/// </summary>
	public class Pcf8563Driver : RtcBase
	{
		/// <summary>
		/// PCF8563 Default I2C Address
		/// </summary>
		public const Byte DefaultI2cAddress = 0x51;

		private I2cDevice _i2cDevice;

		/// <summary>
		/// Creates a new instance of the PCF8563
		/// </summary>
		/// <param name="i2cDevice">The I2C device used for communication.</param>
		public Pcf8563Driver(I2cDevice i2cDevice)
		{
			_i2cDevice = i2cDevice;

			// Set "Normal Mode"
			Span<Byte> ctrl1Config = stackalloc Byte[]
			{
				(Byte)Pcf8563Register.PCF_CTRL1_ADDR,
				0x00
			};
			_i2cDevice.Write(ctrl1Config);
		}

		/// <summary>
		/// Read Time from PCF8563
		/// </summary>
		/// <returns>PCF8563 Time</returns>
		protected override DateTime ReadTime()
		{
			// Sec, Min, Hour, Date, Day, Month & Century, Year
			Span<Byte> readBuffer = stackalloc Byte[7];

			_i2cDevice.WriteByte((Byte)Pcf8563Register.PCF_SEC_ADDR);
			_i2cDevice.Read(readBuffer);

			return new DateTime(1900 + (readBuffer[5] >> 7) * 100 + NumberHelper.Bcd2Dec(readBuffer[6]),
								NumberHelper.Bcd2Dec((Byte)(readBuffer[5] & 0b_0001_1111)),
								NumberHelper.Bcd2Dec((Byte)(readBuffer[3] & 0b_0011_1111)),
								NumberHelper.Bcd2Dec((Byte)(readBuffer[2] & 0b_0011_1111)),
								NumberHelper.Bcd2Dec((Byte)(readBuffer[1] & 0b_0111_1111)),
								NumberHelper.Bcd2Dec((Byte)(readBuffer[0] & 0b_0111_1111)));
		}

		/// <summary>
		/// Set PCF8563 Time
		/// </summary>
		/// <param name="time">Time</param>
		protected override void SetTime(DateTime time)
		{
			Span<Byte> writeBuffer = stackalloc Byte[8];

			writeBuffer[0] = (Byte)Pcf8563Register.PCF_SEC_ADDR;
			// Set bit8 as 0 to guarantee clock integrity
			writeBuffer[1] = (Byte)(NumberHelper.Dec2Bcd(time.Second) & 0b_0111_1111);
			writeBuffer[2] = NumberHelper.Dec2Bcd(time.Minute);
			writeBuffer[3] = NumberHelper.Dec2Bcd(time.Hour);
			writeBuffer[4] = NumberHelper.Dec2Bcd(time.Day);
			writeBuffer[5] = NumberHelper.Dec2Bcd((Int32)time.DayOfWeek);
			if(time.Year >= 2000)
			{
				writeBuffer[6] = (Byte)(NumberHelper.Dec2Bcd(time.Month) | 0b_1000_0000);
				writeBuffer[7] = NumberHelper.Dec2Bcd(time.Year - 2000);
			}
			else
			{
				writeBuffer[6] = NumberHelper.Dec2Bcd(time.Month);
				writeBuffer[7] = NumberHelper.Dec2Bcd(time.Year - 1900);
			}

			_i2cDevice.Write(writeBuffer);
		}

		/// <summary>
		/// Cleanup
		/// </summary>
		protected override void Dispose(Boolean disposing)
		{
			_i2cDevice?.Dispose();
			_i2cDevice = null;

			base.Dispose(disposing);
		}
	}
}
