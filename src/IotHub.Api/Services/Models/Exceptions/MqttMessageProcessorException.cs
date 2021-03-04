using System;
using System.Runtime.Serialization;

namespace IotHub.Api.Services.Models.Exceptions
{
	internal class MqttMessageProcessorException : ApplicationException
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