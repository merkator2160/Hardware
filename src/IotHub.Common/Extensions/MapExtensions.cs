using System;

namespace IotHub.Common.Extensions
{
	public static class MapExtensions
	{
		public static Double Map(this Double value, Double inMin, Double inMax, Double outMin, Double outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
		public static Single Map(this Single value, Single inMin, Single inMax, Single outMin, Single outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
		public static Decimal Map(this Decimal value, Decimal inMin, Decimal inMax, Decimal outMin, Decimal outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
		public static Int64 Map(this Int64 value, Int64 inMin, Int64 inMax, Int64 outMin, Int64 outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
		public static Int32 Map(this Int32 value, Int32 inMin, Int32 inMax, Int32 outMin, Int32 outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
		public static Int16 Map(this Int16 value, Int16 inMin, Int16 inMax, Int16 outMin, Int16 outMax)
		{
			return (Int16)((value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin);
		}

		// Byte
		public static Single Map(this Byte value, Byte inMin, Byte inMax, Single outMin, Single outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
		public static Int32 Map(this Byte value, Byte inMin, Byte inMax, Int32 outMin, Int32 outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
		public static Byte Map(this Byte value, Byte inMin, Byte inMax, Byte outMin, Byte outMax)
		{
			return (Byte)((value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin);
		}
	}
}