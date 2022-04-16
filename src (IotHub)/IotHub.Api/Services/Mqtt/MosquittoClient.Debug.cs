using uPLibrary.Networking.M2Mqtt;

namespace IotHub.Api.Services.Mqtt
{
    internal partial class MosquittoClient
    {
        // TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
        public void AddDebugHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
        {
            //handlerDictionary.Add("unit2/moisture/value", OnUnit2MoistureSensorMessageReceived);
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
    }
}