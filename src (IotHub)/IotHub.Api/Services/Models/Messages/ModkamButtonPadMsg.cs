using Common.Converters;
using Newtonsoft.Json;

namespace IotHub.Api.Services.Models.Messages
{
    internal class ModkamButtonPadMsg
	{
		[JsonProperty("friendly_name")]
		public String FriendlyName { get; set; }

		[JsonProperty("model_name")]
		public String ModelName { get; set; }

		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }

		public String Action { get; set; }      // Null in system messages

		[JsonProperty("battery")]
		public Byte BatteryPercentage { get; set; }

		public Byte LinkQuality { get; set; }

		[JsonProperty("voltage")]
		public Single BatteryVoltage { get; set; }
	}
}