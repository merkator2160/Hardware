using System;
using System.Runtime.Serialization;

namespace Common.Models.Exceptions
{
	public class DeviceException : ApplicationException
	{
		public DeviceException()
		{

		}
		public DeviceException(String message) : base(message)
		{

		}
		public DeviceException(String message, Exception ex) : base(message)
		{

		}
		protected DeviceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}
}