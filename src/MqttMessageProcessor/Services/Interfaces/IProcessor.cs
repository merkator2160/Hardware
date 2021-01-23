using System;

namespace MqttMessageProcessor.Services.Interfaces
{
	internal interface IProcessor : IDisposable
	{
		Boolean IsConnected { get; }

		void Start();
		void Stop();
	}
}