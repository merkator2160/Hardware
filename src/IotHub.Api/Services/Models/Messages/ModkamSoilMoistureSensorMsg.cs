using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class ModkamSoilMoistureSensorMsg
	{
		[JsonProperty("battery")]
		public Byte BatteryPercentage { get; set; }
		public Single Humidity { get; set; }
		public Single Illuminance { get; set; }

		[JsonProperty("last_seen")]
		public Int32 LastSeen { get; set; }
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