using System;
using System.Runtime.Serialization;

namespace Common.Models.Exceptions
{
	public class ControllerNotFoundException : ApplicationException
	{
		public ControllerNotFoundException()
		{

		}
		public ControllerNotFoundException(String message) : base(message)
		{

		}
		public ControllerNotFoundException(String message, Exception ex) : base(message)
		{

		}
		protected ControllerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}
}