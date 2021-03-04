using IotHub.Api.Services.Models.Enums;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models
{
	internal class DomosticzInMessage
	{
		[JsonProperty("idx")]
		public DomosticzDevice DeviceId { get; set; }
		public Int32 Rssi { get; set; }

		[JsonProperty("nvalue")]
		public Int32 NumericValue { get; set; }

		[JsonProperty("svalue")]
		public String StringValue { get; set; }
	}
}