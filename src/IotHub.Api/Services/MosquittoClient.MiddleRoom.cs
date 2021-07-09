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
			handlerDictionary.Add("midRoomClimateSensor/bmp280/temp", OnMiddleRoomClimateSensorTemperatureMessageReceived);
			handlerDictionary.Add("midRoomClimateSensor/bmp280/hum", OnMiddleRoomClimateSensorHumidityMessageReceived);
			handlerDictionary.Add("midRoomClimateSensor/mhz19b/ppm", OnMiddleRoomClimateSensorCo2MessageReceived);
			handlerDictionary.Add("midRoomClimateSensor/pms7003/pm2.5", OnMiddleRoomClimateSensorDustMessageReceived);
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