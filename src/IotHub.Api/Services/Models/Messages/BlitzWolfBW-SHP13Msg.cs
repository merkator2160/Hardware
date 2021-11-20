using IotHub.Common.Converters;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	public class BlitzWolfBW_SHP13Msg
	{
		[JsonProperty("friendly_name")]
		public String FriendlyName { get; set; }

		[JsonProperty("model_name")]
		public String ModelName { get; set; }

		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }

		public Single Current { get; set; }
		public Single Energy { get; set; }
		public Byte LinkQuality { get; set; }
		public Int32 Power { get; set; }
		public String State { get; set; }
		public Single Voltage { get; set; }
	}
}