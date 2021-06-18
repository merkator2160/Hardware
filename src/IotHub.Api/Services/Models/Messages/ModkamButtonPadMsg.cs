using IotHub.Common.Converters;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class ModkamButtonPadMsg
	{
		public String Action { get; set; }      // Null in system messages

		[JsonProperty("battery")]
		public Byte BatteryPercentage { get; set; }

		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }
		public Byte LinkQuality { get; set; }

		[JsonProperty("voltage")]
		public Single BatteryVoltage { get; set; }
	}
}