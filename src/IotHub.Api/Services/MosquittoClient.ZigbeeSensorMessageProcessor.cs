using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using IotHub.Common.Enums;
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
		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddZigbeeHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.LargeRoomThermometer}", OnLargeRoomThermometerMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomThermometer}", OnThermometer1MessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenFikusMoistureSensor}", OnKitchenFikusMoistureSensorMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.ButtonPad12}", OnButtonPad12MessageReceived);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnLargeRoomThermometerMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraThermometerMsg>(jsonStr);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.LargeRoomThermometer,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = $"{message.Temperature};{message.Humidity};{(Byte)DomosticzEnvironmentLevel.Normal};{message.Pressure};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			});
		}
		private void OnThermometer1MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraThermometerMsg>(jsonStr);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.SideRoomThermometer,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = $"{message.Temperature};{message.Humidity};{(Byte)DomosticzEnvironmentLevel.Normal};{message.Pressure};{(Byte)DomosticzBarometerPrediction.NoPrediction}"
			});
		}
		private void OnKitchenFikusMoistureSensorMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamSoilMoistureSensorMsg>(jsonStr);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.FikusLight,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.Illuminance.ToString(CultureInfo.InvariantCulture)
			});
			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.FikusSoilMoisture,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.SoilMoisture.ToString(CultureInfo.InvariantCulture)
			});
		}
		private void OnButtonPad12MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamButtonPadMsg>(jsonStr);

			if(message.Action == null)      // System message
				return;

			if(message.Action.Equals(ModkamButtonPadActions.Button1SingleClick))
				ToggleLed();
		}
	}
}