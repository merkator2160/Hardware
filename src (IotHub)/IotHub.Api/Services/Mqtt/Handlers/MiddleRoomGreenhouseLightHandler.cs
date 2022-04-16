using IotHub.Api.Services.Interfaces;
using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Mqtt.Handlers
{
    internal class MiddleRoomGreenhouseLightHandler : MqttHandlerBase
    {
        private Boolean _isLightEnabled;


        public MiddleRoomGreenhouseLightHandler(IMqttPublisher publisher) : base(publisher)
        {

        }


        // TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
        public override void RegisterHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
        {
            handlerDictionary.Add($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}", OnMiddleRoomGreenhouseLightRelayMessageReceived);
        }



        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void OnMiddleRoomGreenhouseLightRelayMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
            var message = JsonConvert.DeserializeObject<TuyaRelayMsg>(jsonMessage);

            _isLightEnabled = message.State.Equals("ON");
        }



        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void ToggleSideRoomGreenhouseLight()
        {
            _publisher.Publish($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}/set/state", _isLightEnabled ? "OFF" : "ON");
        }
        public void TurnOnSideRoomGreenhouseLight()
        {
            _publisher.Publish($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}/set/state", "ON");
        }
        public void TurnOffSideRoomGreenhouseLight()
        {
            _publisher.Publish($"zigbee/{ZigbeeDevice.MiddleRoomGreenhouseLightRelay}/set/state", "OFF");
        }
    }
}