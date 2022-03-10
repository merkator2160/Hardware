using IotHub.Common.Const.IrController;
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
		public void AddIrHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add("irReceiverMiddleRoom/ir/value", OnIrMessageReceivedMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnIrMessageReceivedMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var value = Int64.Parse(Encoding.UTF8.GetString(eventArgs.Message));

			if(value.Equals(AverTv.Record))
				ToggleMonitorLed();
		}
	}
}