using System;
using System.Buffers.Binary;
using System.Linq;

namespace Common.Helpers
{
	public static class ArrayConverter
	{
		public static Int16[] ToInt16Array(this Span<Byte> buffer)
		{
			return ToInt16Array(buffer.ToArray());
		}
		public static Int16[] ToInt16Array(this Byte[] buffer)
		{
			const Byte intSize = 2;
			var values = new Int16[buffer.Length / intSize];

			for(var i = 0; i < values.Length; i++)
			{
				var currentInt = buffer.Skip(i * intSize).Take(intSize).ToArray();
				values[i] = BinaryPrimitives.ReadInt16BigEndian(currentInt);
			}

			return values;
		}
	}
}