using System;

namespace IotHub.Api.Services.Interfaces
{
	internal interface IMqttMessageProcessor
	{
		Boolean IsConnected { get; }

		void Start();
		void Stop();
	}
}