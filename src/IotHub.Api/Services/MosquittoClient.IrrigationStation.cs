using IotHub.Api.Services.Models.Exceptions;
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
		public void AddWaterPumpHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.IrrigationStation}", OnWaterPumpMessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnWaterPumpMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamWaterPumpMsg>(jsonStr);



			// TODO: Implement water leak detection
			//Publish("domoticz/in", new DomosticzInMsg()
			//{
			//	DeviceId = DomosticzDevice.LargeRoomThermometer,
			//	Rssi = message.LinkQuality,
			//	Battery = message.BatteryPercentage,
			//	StringValue = $"{message.Temperature};{message.Humidity};{(Byte)DomosticzEnvironmentLevel.Normal};{message.Pressure};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			//});
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		private void StartPump(Byte pumpNumber)
		{
			if(pumpNumber < 1 || pumpNumber > 3)
				throw new MqttMessageProcessorException($"Irrigation station has no pump with number {pumpNumber}! Irrigation station has pumps with numbers: 1, 2, 3.");

			Publish($"zigbee/{ZigbeeDevice.IrrigationStation}/set/pump_1", "ON");
		}
	}
}