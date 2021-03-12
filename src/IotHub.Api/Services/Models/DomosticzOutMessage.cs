﻿using IotHub.Api.Services.Models.Enums;
using Newtonsoft.Json;
using System;

namespace IotHub.Api.Services.Models
{
	internal class DomosticzOutMessage
	{
		public Byte Battery { get; set; }
		public Int32 Rssi { get; set; }
		public String Description { get; set; }

		[JsonProperty("dtype")]
		public String DeviceType { get; set; }

		[JsonProperty("hwid")]
		public String HardwareId { get; set; }
		public String Id { get; set; }

		[JsonProperty("idx")]
		public DomosticzDevice DeviceId { get; set; }

		public String Name { get; set; }

		[JsonProperty("nvalue")]
		public Int32 NumericValue { get; set; }

		[JsonProperty("stype")]
		public String StringType { get; set; }

		[JsonProperty("svalue")]
		public String StringValue { get; set; }
		public String SwitchType { get; set; }
		public Int32 Unit { get; set; }
	}
}