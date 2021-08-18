using System;
using System.Text;

namespace Nano.Common.Extensions
{
	public static class EncodingExtensions
	{
		public static String GetString(this Encoding encoding, Byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}
	}
}