using Common.Converters;
using Newtonsoft.Json;

namespace IotHub.Api.Services.Models.Messages
{
    internal class AquaraMotionSensorMsg
	{
		[JsonProperty("friendly_name")]
		public String FriendlyName { get; set; }

		[JsonProperty("model_name")]
		public String ModelName { get; set; }

		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }

		[JsonProperty("battery")]
		public Byte BatteryPercentage { get; set; }

		[JsonProperty("voltage")]
		public Single BatteryVoltage { get; set; }

		public Byte LinkQuality { get; set; }
		public Boolean Occupancy { get; set; }

		[JsonProperty("occupancy_timeout")]
		public Int32 OccupancyTimeoutSec { get; set; }
		public Int32 Illuminance { get; set; }
	}
}