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
		public void AddButtonPad4Handlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.TuyaButtonPad4}", OnTuyaButtonPad4MessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnTuyaButtonPad4MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<TuyaButtonPadMsg>(jsonStr);

			if(message.Action.Equals(TuyaButtonPadEvents.Button1SingleClick))
				ToggleCockroachRepeller();
		}
	}
}