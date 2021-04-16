using System;

namespace IotHub.Api.Services.Models.Messages
{
	internal class AquaraButtonMsg
	{
		public String Action { get; set; }      // Null in system messages
		public Byte Battery { get; set; }
		public Byte LinkQuality { get; set; }
		public Single Voltage { get; set; }
	}
}