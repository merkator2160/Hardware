using IotHub.Api.Services.Models.Messages;
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
		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddSideRoomHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomThermometer}", OnSideRoomThermometerMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnSideRoomThermometerMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraThermometerMsg>(jsonStr);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.SideRoomThermometer,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = $"{message.Temperature};{message.Humidity};{(Byte)DomosticzEnvironmentLevel.Normal};{message.Pressure};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			});
		}
	}
}