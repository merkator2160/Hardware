using Common.Helpers;
using System;
using System.Device.I2c;

namespace Common.Drivers.Rtc.Ds1307
{
	/// <summary>
	/// Realtime Clock DS1307
	/// </summary>
	public class Ds1307Driver : RtcBase
	{
		public const Byte DefaultI2cAddress = 0x68;
		private I2cDevice _i2cDevice;


		public Ds1307Driver() : this(DefaultI2cAddress)
		{

		}
		public Ds1307Driver(Int32 address, Int32 busId = 1)
		{
			_i2cDevice = I2cDevice.Create(new I2cConnectionSettings(busId, address));
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Read Time from DS1307
		/// </summary>
		/// <returns>DS1307 Time</returns>
		protected override DateTime ReadTime()
		{
			Span<Byte> readBuffer = stackalloc Byte[7];

			// Read all registers at the same time
			_i2cDevice.WriteByte((Byte)Ds1307Register.RTC_SEC_REG_ADDR);
			_i2cDevice.Read(readBuffer);

			// Details in the Datasheet P8
			return new DateTime(2000 + NumberHelper.Bcd2Dec(readBuffer[6]),
								NumberHelper.Bcd2Dec(readBuffer[5]),
								NumberHelper.Bcd2Dec(readBuffer[4]),
								NumberHelper.Bcd2Dec(readBuffer[2]),
								NumberHelper.Bcd2Dec(readBuffer[1]),
								NumberHelper.Bcd2Dec((Byte)(readBuffer[0] & 0b_0111_1111)));
		}

		/// <summary>
		/// Set DS1307 Time
		/// </summary>
		/// <param name="time">Time</param>
		protected override void SetTime(DateTime time)
		{
			Span<Byte> writeBuffer = stackalloc Byte[8];

			writeBuffer[0] = (Byte)Ds1307Register.RTC_SEC_REG_ADDR;

			// Details in the Datasheet P8
			// | bit 7: CH | bit 6-0: sec |
			writeBuffer[1] = (Byte)(NumberHelper.Dec2Bcd(time.Second) & 0b_0111_1111);
			writeBuffer[2] = NumberHelper.Dec2Bcd(time.Minute);
			// | bit 7: 0 | bit 6: 12/24 hour | bit 5-0: hour |
			writeBuffer[3] = (Byte)(NumberHelper.Dec2Bcd(time.Hour) & 0b_0011_1111);
			writeBuffer[4] = NumberHelper.Dec2Bcd((Int32)time.DayOfWeek + 1);
			writeBuffer[5] = NumberHelper.Dec2Bcd(time.Day);
			writeBuffer[6] = NumberHelper.Dec2Bcd(time.Month);
			writeBuffer[7] = NumberHelper.Dec2Bcd(time.Year - 2000);

			_i2cDevice.Write(writeBuffer);
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