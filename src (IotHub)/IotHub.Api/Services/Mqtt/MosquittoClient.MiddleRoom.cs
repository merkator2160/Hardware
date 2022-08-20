﻿using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Mqtt
{
    internal partial class MosquittoClient
    {
        private Boolean _isMiddleRoomGreenhouseLightEnabled;


        // TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
        public void AddMiddleRoomHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
        {
            handlerDictionary.Add($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}", OnMiddleRoomGreenhouseLightRelayMessageReceived);
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void OnMiddleRoomGreenhouseLightRelayMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
            var message = JsonConvert.DeserializeObject<TuyaRelayMsg>(jsonMessage);

            _isMiddleRoomGreenhouseLightEnabled = message.State.Equals("ON");
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void ToggleMiddleRoomGreenhouseLight()
        {
            Publish($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}/set/state", _isMiddleRoomGreenhouseLightEnabled ? "OFF" : "ON");
        }
        public void TurnOnMiddleRoomGreenhouseLight()
        {
            Publish($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}/set/state", "ON");
        }
        public void TurnOffMiddleRoomGreenhouseLight()
        {
            Publish($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}/set/state", "OFF");
        }
    }
}