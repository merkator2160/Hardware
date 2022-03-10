using System;

namespace IotHub.Api.Services.Models.Config
{
	internal class ProcessorConfig
	{
		public String HostName { get; set; }
		public Int32 Port { get; set; }
		public String Login { get; set; }
		public String Password { get; set; }
		public String ClientId { get; set; }
	}
}