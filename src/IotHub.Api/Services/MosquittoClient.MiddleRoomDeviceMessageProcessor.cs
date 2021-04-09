using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services
{
	internal partial class MosquittoClient
	{
		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddMiddleRoomDeviceHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add("middleRoomClimateSensor/bmp280/temperature", OnMiddleRoomClimateSensorTemperatureMessageReceived);
			handlerDictionary.Add("middleRoomClimateSensor/bmp280/humidity", OnMiddleRoomClimateSensorHumidityMessageReceived);
			handlerDictionary.Add("middleRoomClimateSensor/co2/ppm", OnMiddleRoomClimateSensorCo2MessageReceived);
			handlerDictionary.Add("middleRoomClimateSensor/dust/pm2.5", OnMiddleRoomClimateSensorDustMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnMiddleRoomClimateSensorTemperatureMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var temperatureStr = Encoding.UTF8.GetString(eventArgs.Message);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.MiddleRoomThermometer,
				StringValue = $"{temperatureStr};{0};{(Byte)DomosticzEnvironmentLevel.Normal};{0};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			});
		}
		private void OnMiddleRoomClimateSensorHumidityMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var humidityStr = Encoding.UTF8.GetString(eventArgs.Message);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.MiddleRoomHumidity,
				StringValue = humidityStr
			});
		}
		private void OnMiddleRoomClimateSensorCo2MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var co2Str = Encoding.UTF8.GetString(eventArgs.Message);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.MiddleRoomCo2,
				StringValue = co2Str
			});
		}
		private void OnMiddleRoomClimateSensorDustMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var dustStr = Encoding.UTF8.GetString(eventArgs.Message);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.MiddleRoomDust,
				StringValue = dustStr
			});
		}
	}
}