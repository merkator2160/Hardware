using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using IotHub.Common.Enums;
using IotHub.Common.Extensions;
using IotHub.Common.Filters;
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
		private KalmanFilter _kalmanFilter;



		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddDebugHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.ButtonPad12}", OnButtonPad12MessageReceived);
			handlerDictionary.Add("unit2/moisture/value", OnUnit2MoistureSensorMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnButtonPad12MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamButtonPadMsg>(jsonStr);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(ModkamButtonPadActions.Button1SingleClick))
				ToggleLed();

			if(message.Action.Equals(ModkamButtonPadActions.Button2SingleClick))
				StartPump(1);

			if(message.Action.Equals(ModkamButtonPadActions.Button3SingleClick))
			{
				StartPump(2);
				_easyEspClient.Unit2PlaySoundAsync("d=10,o=6,b=180,c,e,g").Wait();
				AddDomoticzLog("Gloxinia pump has started manually.");
			}

			if(message.Action.Equals(ModkamButtonPadActions.Button4SingleClick))
				StartPump(3);
		}
		private void OnUnit2MoistureSensorMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var valueStr = Encoding.UTF8.GetString(eventArgs.Message);
			var value = Byte.Parse(valueStr);

			value = value.Map((Byte)12, (Byte)160, (Byte)0, (Byte)100);       // Moisture sensor min: 12, max: 160
			value = value.Constrain(0, 100);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenGloxiniaSoilMoisture,
				StringValue = value.ToString(CultureInfo.InvariantCulture)
			});

			if(value < 90)
			{
				StartPump(2);
				_easyEspClient.Unit2PlaySoundAsync("d=10,o=6,b=180,c,e,g").Wait();
				AddDomoticzLog("Gloxinia pump has started by the moisture sensor.");
			}

			Publish("unit2/moisture/mapped", value);

			_kalmanFilter ??= new KalmanFilter(1f, 0.01f);
			value = (Byte)_kalmanFilter.Calculate((Single)value);
			Publish("unit2/moisture/filtered", value);
		}
	}
}