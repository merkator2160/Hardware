using IotHub.Common.Converters;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class TuyaButtonPadMsg
	{
		[JsonProperty("friendly_name")]
		public String FriendlyName { get; set; }

		[JsonProperty("model_name")]
		public String ModelName { get; set; }

		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }
		public String Action { get; set; }      // It seems there is no system messages available
		public Byte LinkQuality { get; set; }
	}
}