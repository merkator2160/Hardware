using System;
using System.Runtime.Serialization;

namespace Common.Models.Exceptions
{
	public class DeviceNotFoundException : ApplicationException
	{
		public DeviceNotFoundException()
		{

		}
		public DeviceNotFoundException(String message) : base(message)
		{

		}
		public DeviceNotFoundException(String message, Exception ex) : base(message)
		{

		}
		protected DeviceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}
}