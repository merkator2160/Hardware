using System;

namespace IotHub.Api.Services.Models
{
	internal class AquaraThermometerMessage
	{
		public Byte Battery { get; set; }
		public Single Humidity { get; set; }
		public Byte LinkQuality { get; set; }
		public Single Pressure { get; set; }
		public Single Temperature { get; set; }
		public Single Voltage { get; set; }
	}
}