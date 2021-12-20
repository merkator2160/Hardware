using Hangfire;
using IotHub.Api.Services.Interfaces;
using IotHub.Common.Hangfire.Interfaces;
using NLog;
using System;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
	internal class SideRoomKaktusLightRecurringJob : IJob
	{
		private readonly ILogger _logger;
		private readonly ISideRoomMqttLightControl _sideRoomMqttLightControl;


		public SideRoomKaktusLightRecurringJob(ILogger logger, ISideRoomMqttLightControl sideRoomMqttLightControl)
		{
			_logger = logger;
			_sideRoomMqttLightControl = sideRoomMqttLightControl;
		}


		// IJob ///////////////////////////////////////////////////////////////////////////////////
		[AutomaticRetry(Attempts = 0)]
		public void Execute()
		{
			try
			{
				if(!_sideRoomMqttLightControl.IsConnected)
					return;

				var now = DateTime.Now;
				if(now.Hour >= 9 && now.Hour < 23)
				{
					_sideRoomMqttLightControl.TurnOnSideRoomGreenhouseLight();
					return;
				}
				if(now.Hour >= 23 || now.Hour < 9)
				{
					_sideRoomMqttLightControl.TurnOffSideRoomGreenhouseLight();
				}
			}
			catch(Exception ex)
			{
				_logger.Error(ex, $"{ex.Message}\r\n{ex.StackTrace}");
				throw;
			}
		}
	}
}