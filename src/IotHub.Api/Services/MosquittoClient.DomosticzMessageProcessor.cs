using IotHub.Api.Services.Models;
using IotHub.Api.Services.Models.Enums;
using IotHub.Common.Const;
using IotHub.Common.Exceptions;
using Newtonsoft.Json;
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
		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void AddDomosticzHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add("domoticz/in", OnDomosticzInReceived);
			handlerDictionary.Add("domoticz/out", OnDomosticzOutReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnDomosticzInReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<DomosticzInMessage>(jsonMessage);

			if(message.DeviceId == DomosticzDevice.WeatherStation)
				HandleWeatherStationMessage(message);
		}
		private void OnDomosticzOutReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<DomosticzOutMessage>(jsonMessage);

			if(message.DeviceId == DomosticzDevice.ThermometerMyRoom)
				HandleThermometerMyRoomMessage(message);
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private void HandleWeatherStationMessage(DomosticzInMessage message)
		{
			var climateSensorValues = message.StringValue.Split(';');
			var temperature = climateSensorValues[0];
			var humidity = climateSensorValues[1];
			//var unknown = climateSensorValues[2];
			var pressureStr = climateSensorValues[3];
			//var unknown = climateSensorValues[4];

			var formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
			if(!Single.TryParse(pressureStr, NumberStyles.AllowDecimalPoint, formatter, out var pressureGpa))
				throw new ParsingException($"Can't parse \"{nameof(pressureGpa)}\"!");

			var pressureMpl = Math.Round(pressureGpa * Global.PressureCoefficient, 2);

			Publish("domoticz/in", new DomosticzInMessage()
			{
				DeviceId = DomosticzDevice.Pressure,
				Rssi = message.Rssi,
				NumericValue = message.NumericValue,
				StringValue = pressureMpl.ToString(CultureInfo.InvariantCulture)
			});
			Publish("iotHub/goncharova/weather/pressure/mpl", pressureMpl);
			Publish("iotHub/goncharova/weather/temperature", temperature);
			Publish("iotHub/goncharova/weather/humidity", humidity);
			Publish("iotHub/goncharova/weather/pressure/gpa", pressureStr);
		}
		private void HandleThermometerMyRoomMessage(DomosticzOutMessage message)
		{
			Publish("thermometerMiddleRoom/import/led", message.NumericValue);
		}
	}
}