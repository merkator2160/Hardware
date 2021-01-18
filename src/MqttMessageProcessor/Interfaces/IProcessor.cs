using System;

namespace MqttMessageProcessor.Interfaces
{
	internal interface IProcessor : IDisposable
	{
		void Start();
		void Stop();
	}
}