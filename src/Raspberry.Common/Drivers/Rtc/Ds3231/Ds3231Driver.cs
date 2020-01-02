using Common.Const.Units;
using Common.Helpers;
using System;
using System.Device.I2c;

namespace Common.Drivers.Rtc.Ds3231
{
	public class Ds3231Driver : RtcBase
	{
		public const Byte DefaultI2cAddress = 0x68;
		private I2cDevice _i2cDevice;


		public Ds3231Driver() : this(DefaultI2cAddress)
		{

		}
		public Ds3231Driver(Int32 address, Int32 busId = 1)
		{
			_i2cDevice = I2cDevice.Create(new I2cConnectionSettings(busId, address));
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		public Temperature Temperature => Temperature.FromCelsius(ReadTemperature());


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		protected override DateTime ReadTime()
		{
			// Sec, Min, Hour, Day, Date, Month & Century, Year
			Span<Byte> rawData = stackalloc Byte[7];

			_i2cDevice.WriteByte((Byte)Ds3231Register.RTC_SEC_REG_ADDR);
			_i2cDevice.Read(rawData);

			return new DateTime(1900 + (rawData[5] >> 7) * 100 + NumberHelper.Bcd2Dec(rawData[6]),
								NumberHelper.Bcd2Dec((Byte)(rawData[5] & 0b_0001_1111)),
								NumberHelper.Bcd2Dec(rawData[4]),
								NumberHelper.Bcd2Dec(rawData[2]),
								NumberHelper.Bcd2Dec(rawData[1]),
								NumberHelper.Bcd2Dec(rawData[0]));
		}
		protected override void SetTime(DateTime time)
		{
			Span<Byte> setData = stackalloc Byte[8];

			setData[0] = (Byte)Ds3231Register.RTC_SEC_REG_ADDR;

			setData[1] = NumberHelper.Dec2Bcd(time.Second);
			setData[2] = NumberHelper.Dec2Bcd(time.Minute);
			setData[3] = NumberHelper.Dec2Bcd(time.Hour);
			setData[4] = NumberHelper.Dec2Bcd((Int32)time.DayOfWeek + 1);
			setData[5] = NumberHelper.Dec2Bcd(time.Day);
			if(time.Year >= 2000)
			{
				setData[6] = (Byte)(NumberHelper.Dec2Bcd(time.Month) | 0b_1000_0000);
				setData[7] = NumberHelper.Dec2Bcd(time.Year - 2000);
			}
			else
			{
				setData[6] = NumberHelper.Dec2Bcd(time.Month);
				setData[7] = NumberHelper.Dec2Bcd(time.Year - 1900);
			}

			_i2cDevice.Write(setData);
		}
		protected Double ReadTemperature()
		{
			Span<Byte> data = stackalloc Byte[2];

			_i2cDevice.WriteByte((Byte)Ds3231Register.RTC_TEMP_MSB_REG_ADDR);
			_i2cDevice.Read(data);

			// datasheet Temperature part
			return data[0] + (data[1] >> 6) * 0.25;
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected override void Dispose(Boolean disposing)
		{
			_i2cDevice?.Dispose();
			_i2cDevice = null;

			base.Dispose(disposing);
		}
	}
}
