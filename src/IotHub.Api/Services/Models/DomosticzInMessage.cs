using IotHub.Common.Enums;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models
{
	internal class DomosticzInMessage
	{
		[JsonProperty("idx")]
		public DomosticzDevice DeviceId { get; set; }
		public Byte Rssi { get; set; }

		public Byte Battery { get; set; }

		[JsonProperty("nvalue")]
		public Single NumericValue { get; set; }

		[JsonProperty("svalue")]
		public String StringValue { get; set; }
	}
}