using Newtonsoft.Json;
using System;

namespace MqttMessageProcessor.Models
{
	public class DomosticzInMessage
	{
		[JsonProperty("idx")]
		public Int32 DeviceId { get; set; }
		public Int32 Rssi { get; set; }
		public Int32 Nvalue { get; set; }
		public String Svalue { get; set; }
	}
}