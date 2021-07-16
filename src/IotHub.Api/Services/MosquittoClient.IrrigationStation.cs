using IotHub.Api.Services.Models.Exceptions;
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
		public void AddIrrigationStationHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.IrrigationStation}", OnWaterPumpMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenKratonSensor}", OnKitchenKratonSensorMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenKaktusSensor}", OnKitchenKaktusSensorMessage);
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
		private void OnKitchenKratonSensorMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamSoilMoistureSensorMsg>(jsonStr);

			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenKratonLight,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.Illuminance.ToString(CultureInfo.InvariantCulture)
			});
			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenKratonSoilMoisture,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.SoilMoisture.ToString(CultureInfo.InvariantCulture)
			});
			Publish("domoticz/in", new DomosticzInMsg()
			{
				DeviceId = DomosticzDevice.KitchenKratonTemperature,
				Rssi = message.LinkQuality,
				Battery = message.BatteryPercentage,
				StringValue = message.TemperatureDs.ToString(CultureInfo.InvariantCulture)
			});

			if(message.SoilMoisture < 90)
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

			if(message.SoilMoisture < 70)
				StartPump(3);
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Pump 1 - kitchen Kraton;
		/// Pump 2 - kitchen Gloxinia;
		/// Pump 3 - kitchen Kaktus tall;
		/// </summary>
		private void StartPump(Byte pumpNumber)
		{
			if(pumpNumber < 1 || pumpNumber > 3)
				throw new MqttMessageProcessorException($"Irrigation station has no pump with number {pumpNumber}! Irrigation station has pumps with numbers: 1, 2, 3.");

			Publish($"zigbee/{ZigbeeDevice.IrrigationStation}/set/pump_{pumpNumber}", "ON");
		}
	}
}