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
		private Boolean _isKaktusLightEnabled;


		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddSideRoomHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomKaktusLightButton}", OnSideRoomKaktusLightButtonMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomKaktusLightCircuitRelay}", OnSideRoomKaktusLightCircuitRelayMessageReceived);

			//handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomThermometer}", OnSideRoomThermometerMessageReceived);	// Domoticz
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnSideRoomKaktusLightCircuitRelayMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<BlitzWolfBW_SHP13Msg>(jsonMessage);

			_isKaktusLightEnabled = message.State.Equals("ON");
		}
		private void OnSideRoomKaktusLightButtonMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraButtonMsg>(jsonMessage);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(AquaraButtonActions.SingleClick))
				ToggleKaktusLight();
		}
		private void ToggleKaktusLight()
		{
			Publish($"zigbee/{ZigbeeDevice.SideRoomKaktusLightCircuitRelay}/set/state", _isKaktusLightEnabled ? "OFF" : "ON");
		}


		// DOMOTICZ OBSOLETE ///////////////////////////////////////////////////////////////////////////////
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