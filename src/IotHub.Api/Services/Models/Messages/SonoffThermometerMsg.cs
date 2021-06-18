using IotHub.Common.Converters;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class SonoffThermometerMsg
	{
		[JsonProperty("last_seen")]
		[JsonConverter(typeof(PosixDateTimeConverter))]
		public DateTime LastSeen { get; set; }

		public Byte Battery { get; set; }
		public Single Humidity { get; set; }
		public Byte LinkQuality { get; set; }
		public Single Temperature { get; set; }
		public Single Voltage { get; set; }
	}
}