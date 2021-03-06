﻿using IotHub.Common.Converters;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class AquaraThermometerMsg
	{
		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }

		public Single Humidity { get; set; }
		public Byte LinkQuality { get; set; }       // Value range 0-255
		public Single Pressure { get; set; }
		public Single Temperature { get; set; }

		[JsonProperty("battery")]
		public Byte BatteryPercentage { get; set; }

		[JsonProperty("voltage")]
		public Single BatteryVoltage { get; set; }
	}
}