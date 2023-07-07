using Common.Contracts.Const;
using Common.Contracts.Exceptions.Application;
using System.Globalization;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Mqtt
{
    internal partial class MosquittoClient
    {
        // TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
        public void AddWeatherHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
        {
            handlerDictionary.Add("weatherStation/bmp280/press", OnWeatherStationPressureMessageReceived);
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void OnWeatherStationPressureMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            var pressureStr = Encoding.UTF8.GetString(eventArgs.Message);

            var formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            if (!Single.TryParse(pressureStr, NumberStyles.AllowDecimalPoint, formatter, out var pressureGpa))
                throw new ParsingException($"Can't parse \"{nameof(pressureGpa)}\"!");

            var pressureMpl = Math.Round(pressureGpa * Global.PressureCoefficient, 2);

            Publish("iotHub/goncharova/weather/pressure/gpa", pressureStr);
            Publish("iotHub/goncharova/weather/pressure/mpl", pressureMpl);
        }
    }
}