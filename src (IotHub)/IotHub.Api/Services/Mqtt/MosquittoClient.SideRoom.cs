﻿using Common.Contracts.Const;
using IotHub.Api.Services.Interfaces;
using IotHub.Api.Services.Models.Messages;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Mqtt
{
    internal partial class MosquittoClient : IGreenhouseMqttLightControl
    {
        private Boolean _isKaktusLightEnabled;


        // TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
        public void AddSideRoomHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
        {
            handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoom.GreenhouseLightCircuitRelay}", OnSideRoomKaktusLightCircuitRelayMessageReceived);
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
            Publish($"zigbee/{ZigbeeDevice.SideRoom.GreenhouseLightCircuitRelay}/set/state", _isKaktusLightEnabled ? "OFF" : "ON");
        }
        public void TurnOnSideRoomGreenhouseLight()
        {
            Publish($"zigbee/{ZigbeeDevice.SideRoom.GreenhouseLightCircuitRelay}/set/state", "ON");
        }
        public void TurnOffSideRoomGreenhouseLight()
        {
            Publish($"zigbee/{ZigbeeDevice.SideRoom.GreenhouseLightCircuitRelay}/set/state", "OFF");
        }
    }
}