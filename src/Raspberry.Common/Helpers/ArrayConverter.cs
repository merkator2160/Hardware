using System;
using System.Linq;

namespace Common.Helpers
{
	public static class ArrayConverter
	{
		public static Int16[] ToInt16(this Byte[] buffer)
		{
			const Byte intSize = 2;
			var values = new Int16[buffer.Length / intSize];

			for(var i = 0; i < values.Length; i++)
			{
				var currentInt = buffer.Skip(i * intSize).Take(intSize).ToArray();
				values[i] = BitConverter.ToInt16(currentInt, 0);
			}

			return values;
		}
	}
}