using Hangfire;
using IotHub.Api.Services.Interfaces;
using IotHub.Common.Hangfire.Interfaces;
using NLog;
using System;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
	internal class SideRoomKaktusLightTurnOffJob : IJob
	{
		private readonly ILogger _logger;
		private readonly ISideRoomMqttLightControl _sideRoomMqttLightControl;


		public SideRoomKaktusLightTurnOffJob(ILogger logger, ISideRoomMqttLightControl sideRoomMqttLightControl)
		{
			_logger = logger;
			_sideRoomMqttLightControl = sideRoomMqttLightControl;
		}


		// IJob ///////////////////////////////////////////////////////////////////////////////////
		[AutomaticRetry(Attempts = 10)]
		public void Execute()
		{
			try
			{
				_sideRoomMqttLightControl.TurnOffSideRoomGreenhouseLight();
			}
			catch(Exception ex)
			{
				_logger.Error(ex, $"{ex.Message}\r\n{ex.StackTrace}");
				throw;
			}
		}
	}
}