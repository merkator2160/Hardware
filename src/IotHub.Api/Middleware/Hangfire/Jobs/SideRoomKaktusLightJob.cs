using Hangfire;
using IotHub.Api.Services.Interfaces;
using IotHub.Common.Const;
using IotHub.Common.Hangfire.Interfaces;
using NLog;
using System;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
	internal class SideRoomKaktusLightJob : IJob
	{
		private readonly IMqttPublisher _mqttPublisher;
		private readonly ILogger _logger;


		public SideRoomKaktusLightJob(IMqttPublisher mqttPublisher, ILogger logger)
		{
			_mqttPublisher = mqttPublisher;
			_logger = logger;
		}


		// IJob ///////////////////////////////////////////////////////////////////////////////////
		[AutomaticRetry(Attempts = 0)]
		public void Execute()
		{
			try
			{
				if(!_mqttPublisher.IsConnected)
					return;

				var now = DateTime.Now;
				if(now.Hour >= 9 && now.Hour < 23)
				{
					_mqttPublisher.Publish($"zigbee/{ZigbeeDevice.SideRoomKaktusLightCircuitRelay}/set/state", "ON");
					return;
				}
				if(now.Hour >= 23 || now.Hour < 9)
				{
					_mqttPublisher.Publish($"zigbee/{ZigbeeDevice.SideRoomKaktusLightCircuitRelay}/set/state", "OFF");
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