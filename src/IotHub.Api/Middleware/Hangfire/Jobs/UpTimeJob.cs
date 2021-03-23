using Hangfire;
using IotHub.Api.Services.Interfaces;
using IotHub.Common.Hangfire.Interfaces;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
	internal class UpTimeJob : IJob
	{
		private readonly IMqttPublisher _mqttPublisher;

		private static readonly DateTime _startDate;


		static UpTimeJob()
		{
			_startDate = DateTime.Now;
		}
		public UpTimeJob(IMqttPublisher mqttPublisher)
		{
			_mqttPublisher = mqttPublisher;
		}


		// IJob ///////////////////////////////////////////////////////////////////////////////////
		[AutomaticRetry(Attempts = 0)]
		public void Execute()
		{
			if(_mqttPublisher.IsConnected)
			{
				_mqttPublisher.Publish("iotHub/upTime", (DateTime.Now - _startDate).ToString(@"dd\:hh\:mm\:ss"), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
			}
		}
	}
}