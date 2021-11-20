using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using IotHub.Common.Enums;
using IotHub.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services
{
	internal partial class MosquittoClient
	{
		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddWeatherHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			//handlerDictionary.Add("weatherStation/bmp280/temp", OnWeatherStationTemperatureMessageReceived);
			//handlerDictionary.Add("weatherStation/bmp280/hum", OnWeatherStationHumidityMessageReceived);
			handlerDictionary.Add("weatherStation/bmp280/press", OnWeatherStationPressureMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnWeatherStationPressureMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var pressureStr = Encoding.UTF8.GetString(eventArgs.Message);

			var formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
			if(!Single.TryParse(pressureStr, NumberStyles.AllowDecimalPoint, formatter, out var pressureGpa))
				throw new ParsingException($"Can't parse \"{nameof(pressureGpa)}\"!");

			var pressureMpl = Math.Round(pressureGpa * Global.PressureCoefficient, 2);

			Publish("iotHub/goncharova/weather/pressure/gpa", pressureStr);
			Publish("iotHub/goncharova/weather/pressure/mpl", pressureMpl);
		}


		// DOMOTICZ OBSOLETE ///////////////////////////////////////////////////////////////////////////////
		private void OnWeatherStationTemperatureMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var temperatureStr = Encoding.UTF8.GetString(eventArgs.Message);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.WeatherStationTemperature,
				StringValue = $"{temperatureStr};{0};{(Byte)DomosticzEnvironmentLevel.Normal};{0};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			});
		}
		private void OnWeatherStationHumidityMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var humidityValueStr = Encoding.UTF8.GetString(eventArgs.Message);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.WeatherStationHumidity,
				StringValue = humidityValueStr
			});
		}
	}
}