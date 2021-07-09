using System;

namespace IotHub.Common.Extensions
{
	public static class ConstrainExtensions
	{
		public static Double Constrain(this Double value, Double min, Double max)
		{
			if(value < min)
				return min;

			if(value > max)
				return max;

			return value;
		}
		public static Single Constrain(this Single value, Single min, Single max)
		{
			if(value < min)
				return min;

			if(value > max)
				return max;

			return value;
		}
		public static Decimal Constrain(this Decimal value, Decimal min, Decimal max)
		{
			if(value < min)
				return min;

			if(value > max)
				return max;

			return value;
		}
		public static Int32 Constrain(this Int32 value, Int32 min, Int32 max)
		{
			if(value < min)
				return min;

			if(value > max)
				return max;

			return value;
		}
		public static Byte Constrain(this Byte value, Byte min, Byte max)
		{
			if(value < min)
				return min;

			if(value > max)
				return max;

			return value;
		}
	}
}