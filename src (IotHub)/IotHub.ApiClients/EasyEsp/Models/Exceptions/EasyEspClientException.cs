using System;
using System.Runtime.Serialization;

namespace IotHub.ApiClients.EasyEsp.Models.Exceptions
{
	public class EasyEspClientException : ApplicationException
	{
		public EasyEspClientException()
		{

		}
		public EasyEspClientException(String message) : base(message)
		{

		}
		public EasyEspClientException(String message, Exception ex) : base(message)
		{

		}
		protected EasyEspClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}
}