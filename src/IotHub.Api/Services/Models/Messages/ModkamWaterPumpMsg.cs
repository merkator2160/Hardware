using IotHub.Common.Converters;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class ModkamWaterPumpMsg
	{
		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }
		public Byte LinkQuality { get; set; }


		[JsonProperty("pump_1")]
		public String Pump1State { get; set; }

		[JsonProperty("pump_2")]
		public String Pump2State { get; set; }

		[JsonProperty("pump_3")]
		public String Pump3State { get; set; }


		[JsonProperty("pump_timeout_1")]
		public Byte Pump1TimeOut { get; set; }

		[JsonProperty("pump_timeout_2")]
		public Byte Pump2TimeOut { get; set; }

		[JsonProperty("pump_timeout_3")]
		public Byte Pump3TimeOut { get; set; }


		[JsonProperty("water_leak")]
		public Boolean WaterLeak { get; set; }

		[JsonProperty("water_level")]
		public Boolean WaterLevel { get; set; }
	}
}