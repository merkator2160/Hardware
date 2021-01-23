using System;
using System.Runtime.Serialization;

namespace MqttMessageProcessor.Services.Models.Exceptions
{
	public class MqttMessageProcessorException : ApplicationException
	{
		public MqttMessageProcessorException()
		{

		}
		public MqttMessageProcessorException(String message) : base(message)
		{

		}
		public MqttMessageProcessorException(String message, Exception ex) : base(message)
		{

		}
		protected MqttMessageProcessorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}
}