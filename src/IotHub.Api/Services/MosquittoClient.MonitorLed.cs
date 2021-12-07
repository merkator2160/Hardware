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
		public void AddMonitorLedHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add("monitor/led", OnMonitorLedStateChangeConfirmationReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnMonitorLedStateChangeConfirmationReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			_ledState = Byte.Parse(Encoding.UTF8.GetString(eventArgs.Message));
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		private void ToggleMonitorLed()
		{
			Publish("monitor/import/led", _ledState == 1 ? 0 : 1);
		}
	}
}