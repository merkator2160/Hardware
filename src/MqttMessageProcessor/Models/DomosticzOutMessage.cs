using Newtonsoft.Json;
using System;

namespace MqttMessageProcessor.Models
{
	public class DomosticzOutMessage
	{
		[JsonProperty("idx")]
		public Int32 DeviceId { get; set; }
		public Int32 Nvalue { get; set; }
		public String Description { get; set; }
	}
}