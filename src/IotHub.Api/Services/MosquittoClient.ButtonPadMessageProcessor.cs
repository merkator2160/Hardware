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
		public void AddButtonHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.ButtonPad12}", OnButtonPad12MessageReceived);
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
				StartPump(2);

			if(message.Action.Equals(ModkamButtonPadActions.Button4SingleClick))
				StartPump(3);
		}
	}
}