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
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.TuyaButtonPad4}", OnTuyaButtonPad4MessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.ModkamButtonPad12}", OnButtonPad12MessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomKaktusLightButton}", OnSideRoomKaktusLightButtonMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.Button1}", OnButton1MessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnTuyaButtonPad4MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<TuyaButtonPadMsg>(jsonStr);

			if(message.Action.Equals(TuyaButtonPadEvents.Button1SingleClick))
			{
				ToggleMonitorLed();

				return;
			}

			if(message.Action.Equals(TuyaButtonPadEvents.Button2SingleClick))
			{
				ToggleCockroachRepeller();
				return;
			}

			if(message.Action.Equals(TuyaButtonPadEvents.Button3SingleClick))
			{
				ToggleSideRoomKaktusLight();
				return;
			}
		}
		private void OnButtonPad12MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamButtonPadMsg>(jsonStr);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(ModkamButtonPadEvents.Button1SingleClick))
				ToggleMonitorLed();

			if(message.Action.Equals(ModkamButtonPadEvents.Button2SingleClick))
				StartPump(1);

			if(message.Action.Equals(ModkamButtonPadEvents.Button3SingleClick))
			{
				StartPump(2);
				_easyEspClient.Unit2PlaySoundAsync("d=10,o=6,b=180,c,e,g").Wait();
			}

			if(message.Action.Equals(ModkamButtonPadEvents.Button4SingleClick))
				StartPump(3);
		}
		private void OnSideRoomKaktusLightButtonMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraButtonMsg>(jsonMessage);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(AquaraButtonEvents.SingleClick))
				ToggleSideRoomKaktusLight();
		}
		private void OnButton1MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraButtonMsg>(jsonMessage);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(AquaraButtonEvents.SingleClick))
				ToggleMonitorLed();
		}
	}
}