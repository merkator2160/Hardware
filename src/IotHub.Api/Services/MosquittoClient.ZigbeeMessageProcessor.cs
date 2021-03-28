using IotHub.Api.Services.Models;
using IotHub.Common.Const;
using IotHub.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services
{
	internal partial class MosquittoClient
	{
		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void AddZigbeeHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.LargeRoomThermometer}", OnLargeRoomThermometerMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.MiddleRoomThermometer}", OnMiddleRoomThermometerMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnLargeRoomThermometerMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraThermometerMessage>(jsonStr);

			Publish("domoticz/in", new DomosticzInMessage()
			{
				DeviceId = DomosticzDevice.ThermometerLargeRoom,
				Rssi = message.LinkQuality,
				Battery = message.Battery,
				StringValue = $"{message.Temperature};{message.Humidity};{(Byte)DomosticzEnvironmentLevel.Normal};{message.Pressure};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			});
		}
		private void OnMiddleRoomThermometerMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraThermometerMessage>(jsonStr);

			Publish("domoticz/in", new DomosticzInMessage()
			{
				DeviceId = DomosticzDevice.ThermometerMiddleRoom,
				Rssi = message.LinkQuality,
				Battery = message.Battery,
				StringValue = $"{message.Temperature};{message.Humidity};{(Byte)DomosticzEnvironmentLevel.Normal};{message.Pressure};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			});
		}
	}
}