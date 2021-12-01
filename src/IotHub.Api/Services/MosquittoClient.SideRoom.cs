using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
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
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnSideRoomKaktusLightCircuitRelayMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<BlitzCircuitSwitchMsg>(jsonMessage);

			_isKaktusLightEnabled = message.State.Equals("ON");
		}
		private void OnSideRoomKaktusLightButtonMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraButtonMsg>(jsonMessage);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(AquaraButtonEvents.SingleClick))
				ToggleKaktusLight();
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		private void ToggleKaktusLight()
		{
			Publish($"zigbee/{ZigbeeDevice.SideRoomKaktusLightCircuitRelay}/set/state", _isKaktusLightEnabled ? "OFF" : "ON");
		}
	}
}