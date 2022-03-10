﻿using IotHub.Api.Services.Interfaces;
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
	internal partial class MosquittoClient : ISideRoomMqttLightControl
	{
		private Boolean _isKaktusLightEnabled;


		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddSideRoomHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomGreenhouseCircuitRelay}", OnSideRoomKaktusLightCircuitRelayMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnSideRoomKaktusLightCircuitRelayMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<BlitzCircuitSwitchMsg>(jsonMessage);

			_isKaktusLightEnabled = message.State.Equals("ON");
		}



		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void ToggleSideRoomGreenhouseLight()
		{
			Publish($"zigbee/{ZigbeeDevice.SideRoomGreenhouseCircuitRelay}/set/state", _isKaktusLightEnabled ? "OFF" : "ON");
		}
		public void TurnOnSideRoomGreenhouseLight()
		{
			Publish($"zigbee/{ZigbeeDevice.SideRoomGreenhouseCircuitRelay}/set/state", "ON");
		}
		public void TurnOffSideRoomGreenhouseLight()
		{
			Publish($"zigbee/{ZigbeeDevice.SideRoomGreenhouseCircuitRelay}/set/state", "OFF");
		}
	}
}