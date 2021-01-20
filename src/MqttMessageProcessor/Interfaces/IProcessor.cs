using System;

namespace MqttMessageProcessor.Interfaces
{
	internal interface IProcessor : IDisposable
	{
		Boolean IsConnected { get; }

		void Start();
		void Stop();
	}
}