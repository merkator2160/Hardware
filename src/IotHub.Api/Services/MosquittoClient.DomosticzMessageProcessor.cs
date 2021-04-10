using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using IotHub.Common.Enums;
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
	/// <summary>
	/// Domoticz message structure: https://piandmore.wordpress.com/2019/02/04/mqtt-out-for-domoticz/
	/// </summary>
	internal partial class MosquittoClient
	{
		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddDomosticzHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			//handlerDictionary.Add("domoticz/in", OnDomosticzInReceived);		// sample
			handlerDictionary.Add("domoticz/out", OnDomosticzOutReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnDomosticzInReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<DomosticzInMsg>(jsonMessage);

			//if(message.DeviceId == DomosticzDevice.WeatherStation)
			//	HandleWeatherStationMessage(message);
		}           // sample
		private void HandleWeatherStationMessage(DomosticzInMsg message)
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

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.WeatherStationPressure,
				Rssi = message.Rssi,
				StringValue = pressureMpl.ToString(CultureInfo.InvariantCulture)
			});
			Publish("iotHub/goncharova/weather/pressure/mpl", pressureMpl);
			Publish("iotHub/goncharova/weather/temperature", temperature);
			Publish("iotHub/goncharova/weather/humidity", humidity);
			Publish("iotHub/goncharova/weather/pressure/gpa", pressureStr);
		}                               // sample

		private void OnDomosticzOutReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<DomosticzOutMsg>(jsonMessage);

			switch(message.DeviceId)
			{
				case DomosticzDevice.LedSwitch:
					Publish("monitor/import/led", message.NumericValue);
					break;
			}
		}
	}
}