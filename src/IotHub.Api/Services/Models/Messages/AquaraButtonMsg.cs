using IotHub.Common.Converters;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class AquaraButtonMsg
	{
		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }

		public String Action { get; set; }      // Null in system messages
		public Byte Battery { get; set; }
		public Byte LinkQuality { get; set; }
		public Single Voltage { get; set; }
	}
}