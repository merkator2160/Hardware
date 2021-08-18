using nanoFramework.Json;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Text;

namespace Nano.Common.Extensions
{
	public static class MqttClientExtensions
	{
		public static void Publish<T>(this MqttClient mqttClient, String topic, T obj, MqttQoSLevel qos = MqttQoSLevel.AtMostOnce, Boolean retain = false)
		{
			mqttClient.Publish(topic, JsonConvert.SerializeObject(obj), qos, retain);
		}
		public static void Publish(this MqttClient mqttClient, String topic, String message, MqttQoSLevel qos = MqttQoSLevel.AtMostOnce, Boolean retain = false)
		{
			mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
		}
	}
}