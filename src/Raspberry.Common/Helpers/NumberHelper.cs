using System;

namespace Common.Helpers
{
	/// <summary>
	/// Helpers for number.
	/// </summary>
	internal static class NumberHelper
	{
		/// <summary>
		/// BCD To decimal
		/// </summary>
		/// <param name="bcd">BCD Code</param>
		/// <returns>decimal</returns>
		public static Int32 Bcd2Dec(Byte bcd)
		{
			return ((bcd >> 4) * 10) + (bcd % 16);
		}

		/// <summary>
		/// BCD To decimal
		/// </summary>
		/// <param name="bcds">BCD Code</param>
		/// <returns>decimal</returns>
		public static Int32 Bcd2Dec(Byte[] bcds)
		{
			Int32 result = 0;
			foreach(Byte bcd in bcds)
			{
				result *= 100;
				result += Bcd2Dec(bcd);
			}

			return result;
		}

		/// <summary>
		/// Decimal To BCD
		/// </summary>
		/// <param name="dec">decimal</param>
		/// <returns>BCD Code</returns>
		public static Byte Dec2Bcd(Int32 dec)
		{
			if((dec > 99) || (dec < 0))
			{
				throw new ArgumentException($"{nameof(dec)}, encoding value can't be more than 99");
			}

			return (Byte)(((dec / 10) << 4) + (dec % 10));
		}
	}
}
