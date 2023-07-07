using Common.Converters;
using Newtonsoft.Json;

namespace IotHub.Api.Services.Models.Messages
{
    internal class ModkamSoilMoistureSensorMsg
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
		public Single Humidity { get; set; }
		public Single Illuminance { get; set; }

		public Byte LinkQuality { get; set; }
		public Single Pressure { get; set; }

		[JsonProperty("soil_moisture")]
		public Single SoilMoisture { get; set; }

		[JsonProperty("temperature_bme")]
		public Single TemperatureBme { get; set; }

		[JsonProperty("temperature_ds")]
		public Single TemperatureDs { get; set; }

		[JsonProperty("voltage")]
		public Single BatteryVoltage { get; set; }
	}
}