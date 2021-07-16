using System;

namespace IotHub.Api.Services.Interfaces
{
	public interface IDomoticzLogger
	{
		void AddDomoticzLog(String message);
	}
}