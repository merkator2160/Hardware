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
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenFikusSensor}", OnKitchenFikusSensorMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenKaktusSensor}", OnKitchenKaktusSensorMessage);
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
		private void OnKitchenFikusSensorMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamSoilMoistureSensorMsg>(jsonStr);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenFikusLight,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.Illuminance.ToString(CultureInfo.InvariantCulture)
			});
			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenFikusSoilMoisture,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.SoilMoisture.ToString(CultureInfo.InvariantCulture)
			});
			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenFikusTemperature,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.TemperatureDs.ToString(CultureInfo.InvariantCulture)
			});

			if(message.SoilMoisture < 80F)
				StartPump(1);
		}
		private void OnKitchenKaktusSensorMessage(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamSoilMoistureSensorMsg>(jsonStr);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenKaktusLight,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.Illuminance.ToString(CultureInfo.InvariantCulture)
			});
			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenKaktusSoilMoisture,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.SoilMoisture.ToString(CultureInfo.InvariantCulture)
			});
		}
	}
}