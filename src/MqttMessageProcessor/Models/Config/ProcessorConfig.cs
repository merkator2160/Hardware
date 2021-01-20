using System;

namespace MqttMessageProcessor.Models.Config
{
	public class ProcessorConfig
	{
		public String IpAddress { get; set; }
		public Int32 Port { get; set; }
		public String Login { get; set; }
		public String Password { get; set; }
	}
}