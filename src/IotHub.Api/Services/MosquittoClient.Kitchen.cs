using IotHub.Api.Services.Models.Exceptions;
using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using IotHub.Common.Extensions;
using IotHub.Common.Filters;
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
		private KalmanFilter _kalmanFilter;
		private Boolean _isCockroachRepellerEnabled;


		// TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
		public void AddKitchenHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
		{
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.IrrigationStation}", OnWaterPumpMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenKratonSensor}", OnKitchenKratonSensorMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenKaktusSensor}", OnKitchenKaktusSensorMessage);
			handlerDictionary.Add("unit2/moisture/value", OnUnit2MoistureSensorMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenMotionSensor}", OnKitchenMotionSensorMessageReceived);
			handlerDictionary.Add($"zigbee/{ZigbeeDevice.KitchenTornadoUltrasonicCockroachRepellerSwitch}", OnCockroachRepellerMessageReceived);
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

			if(message.SoilMoisture < 90)
			{
				IrrigationStation2StartPump(1, 10);
				//IrrigationStation1StartPump(1);
			}
		}
		private void OnKitchenKaktusSensorMessage(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<ModkamSoilMoistureSensorMsg>(jsonStr);

			if(message.SoilMoisture < 70)
			{
				IrrigationStation2StartPump(2, 10);
				//IrrigationStation1StartPump(3);
			}
		}
		private void OnUnit2MoistureSensorMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var valueStr = Encoding.UTF8.GetString(eventArgs.Message);
			var value = Byte.Parse(valueStr);

			value = value.Map((Byte)12, (Byte)160, (Byte)0, (Byte)100);       // Moisture sensor min: 12, max: 160
			value = value.Constrain(0, 100);

			if(value < 50)
			{
				IrrigationStation1StartPump(2);
				_easyEspClient.Unit2PlaySoundAsync("d=10,o=6,b=180,c,e,g").Wait();
			}

			Publish("unit2/moisture/mapped", value);

			_kalmanFilter ??= new KalmanFilter(1f, 0.01f);
			value = (Byte)_kalmanFilter.Calculate((Single)value);
			Publish("unit2/moisture/filtered", value);
		}
		private void OnKitchenMotionSensorMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<AquaraMotionSensorMsg>(jsonMessage);

			if(message.Occupancy)
			{
				DisableCockroachRepeller();
				return;
			}

			EnableCockroachRepeller();
		}
		private void OnCockroachRepellerMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<SonoffSwitchMsg>(jsonMessage);

			_isCockroachRepellerEnabled = message.State.Equals("ON");
		}



		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Pump 1 - kitchen Kraton;
		/// Pump 2 - kitchen Gloxinia;
		/// Pump 3 - kitchen Kaktus tall;
		/// </summary>
		private void IrrigationStation1StartPump(Byte pumpNumber)
		{
			if(pumpNumber < 1 || pumpNumber > 3)
				throw new MqttMessageProcessorException($"Irrigation station 1 has no pump with number {pumpNumber}! Irrigation station has pumps with numbers: 1, 2, 3.");

			Publish($"zigbee/{ZigbeeDevice.IrrigationStation}/set/pump_{pumpNumber}", "ON");
		}
		private void IrrigationStation2StartPump(Byte pumpNumber, Int32 durationSec)
		{
			if(pumpNumber < 1 || pumpNumber > 2)
				throw new MqttMessageProcessorException($"Irrigation station 2 has no pump with number {pumpNumber}! Irrigation station has pumps with numbers: 1, 2.");

			Publish($"kitchenIrrigationStation2/pump{pumpNumber}/set", durationSec.ToString());
		}

		private void ToggleCockroachRepeller()
		{
			Publish($"zigbee/{ZigbeeDevice.KitchenTornadoUltrasonicCockroachRepellerSwitch}/set/state", _isCockroachRepellerEnabled ? "OFF" : "ON");
		}
		private void EnableCockroachRepeller()
		{
			Publish($"zigbee/{ZigbeeDevice.KitchenTornadoUltrasonicCockroachRepellerSwitch}/set/state", "ON");
		}
		private void DisableCockroachRepeller()
		{
			Publish($"zigbee/{ZigbeeDevice.KitchenTornadoUltrasonicCockroachRepellerSwitch}/set/state", "OFF");
		}
	}
}