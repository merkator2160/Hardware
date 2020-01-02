﻿
namespace Common.Drivers.Rtc.Ds1307
{
	/// <summary>
	/// Register of DS1307
	/// </summary>
	internal enum Ds1307Register : byte
	{
		RTC_SEC_REG_ADDR = 0x00,
		RTC_MIN_REG_ADDR = 0x01,
		RTC_HOUR_REG_ADDR = 0x02,
		RTC_DAY_REG_ADDR = 0x03,
		RTC_DATE_REG_ADDR = 0x04,
		RTC_MONTH_REG_ADDR = 0x05,
		RTC_YEAR_REG_ADDR = 0x06,
		RTC_CTRL_ADDR = 0x07,
	}
}
