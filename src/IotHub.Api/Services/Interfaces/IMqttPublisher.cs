using System;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Interfaces
{
	internal interface IMqttPublisher
	{
		void Publish<T>(String topic, T obj, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false);
		void Publish(String topic, String message, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false);
	}
}