using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using IotHub.Common.Enums;
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
		private Byte _ledState;


		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddLedHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add("monitor/led", OnMonitorLedStateChangeConfirmationReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.Button1}", OnButton1MessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnDomoticzLedMessageReceived(DomosticzOutMsg message)
		{
			Publish("monitor/import/led", (Byte)message.NumericValue);
		}
		private void OnMonitorLedStateChangeConfirmationReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			_ledState = Byte.Parse(Encoding.UTF8.GetString(eventArgs.Message));

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.LedSwitch,
				NumericValue = _ledState
			});
		}
		private void OnButton1MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraButtonMsg>(jsonMessage);

			if(message.Action == null)
				return;

			if(message.Action.Equals(AquaraButtonActions.SingleClick))
			{
				if(_ledState == 1)
				{
					_ledState = 0;
					Publish("monitor/import/led", _ledState);
				}
				else
				{
					_ledState = 1;
					Publish("monitor/import/led", _ledState);
				}
			}
		}
	}
}
