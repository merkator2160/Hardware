using System;
using System.Runtime.Serialization;

namespace IotHub.Common.Exceptions
{
	public class ParsingException : ApplicationException
	{
		public ParsingException()
		{

		}
		public ParsingException(String message) : base(message)
		{

		}
		public ParsingException(String message, Exception ex) : base(message)
		{

		}
		protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}
}