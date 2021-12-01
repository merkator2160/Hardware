using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services
{
	internal partial class MosquittoClient
	{
		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddButtonPad12Handlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.ModkamButtonPad12}", OnButtonPad12MessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnButtonPad12MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamButtonPadMsg>(jsonStr);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(ModkamButtonPadEvents.Button1SingleClick))
				ToggleLed();

			if(message.Action.Equals(ModkamButtonPadEvents.Button2SingleClick))
				StartPump(1);

			if(message.Action.Equals(ModkamButtonPadEvents.Button3SingleClick))
			{
				StartPump(2);
				_easyEspClient.Unit2PlaySoundAsync("d=10,o=6,b=180,c,e,g").Wait();
				AddDomoticzLog("Gloxinia pump has started manually.");
			}

			if(message.Action.Equals(ModkamButtonPadEvents.Button4SingleClick))
				StartPump(3);
		}
	}
}