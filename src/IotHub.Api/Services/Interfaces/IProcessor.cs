using System;

namespace IotHub.Api.Services.Interfaces
{
	internal interface IProcessor : IDisposable
	{
		Boolean IsConnected { get; }

		void Start();
		void Stop();
	}
}