using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class ZigbeeGatewayConfigMsg
	{
		public Int64 UpTime { get; set; }
		public String UpTimeStr { get; set; }

		[JsonProperty("IP")]
		public String IpAddress { get; set; }

		[JsonProperty("RSSI")]
		public String Rssi { get; set; }
		public String Version { get; set; }

		[JsonProperty("FreeMem")]
		public Int64 FreeMemory { get; set; }

		[JsonProperty("LastRxTS")]
		public Int64 LastRxTimeStamp { get; set; }

		[JsonProperty("LastTxTS")]
		public Int64 LastTxTimeStamp { get; set; }

		[JsonProperty("log_level")]
		public String LogLevel { get; set; }

		[JsonProperty("permit_join")]
		public Boolean PermitJoin { get; set; }
	}
}