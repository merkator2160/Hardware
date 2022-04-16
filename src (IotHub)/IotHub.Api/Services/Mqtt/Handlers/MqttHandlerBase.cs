using IotHub.Api.Services.Interfaces;
using uPLibrary.Networking.M2Mqtt;

namespace IotHub.Api.Services.Mqtt.Handlers
{
    internal abstract class MqttHandlerBase
    {
        protected readonly IMqttPublisher _publisher;


        protected MqttHandlerBase(IMqttPublisher publisher)
        {
            _publisher = publisher;
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public abstract void RegisterHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary);
    }
}