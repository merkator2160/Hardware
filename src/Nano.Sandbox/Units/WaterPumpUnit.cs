using Nano.Common.Extensions;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Nano.Sandbox.Units
{
	internal static class WaterPumpUnit
	{
		private const String _brokerAddress = "192.168.1.11";
		private const String _mqttClientId = "588b1499-2f29-4c8a-82ef-178d26b56c5d";
		private const String _mqttServerUserName = "user";
		private const String _mqttServerPassword = ";4,wnmCvQ.7hMDdWuqv*";
		private const String _statusTopic = "esp32/status";
		private const String _pumpTopic = "esp32/pump";
		private const String _ledTopic = "esp32/led";
		private const MqttQoSLevel _qos = MqttQoSLevel.ExactlyOnce;
		private const Int32 _pumpActiveInterval = 5000;

		private static MqttClient _mqttClient;
		private static GpioController _ledPinController;
		private static GpioController _pumpMosPinController;
		private static GpioPin _ledPin;
		private static GpioPin _pumpPin;


		public static void Run()
		{
			_ledPinController = new GpioController();
			_pumpMosPinController = new GpioController();

			_ledPin = _ledPinController.OpenPin(14, PinMode.Output);
			_pumpPin = _pumpMosPinController.OpenPin(12, PinMode.Output);

			_ledPin.Write(PinValue.Low);
			_pumpPin.Write(PinValue.Low);

			_mqttClient = new MqttClient(_brokerAddress, 1883, false, null, null, MqttSslProtocols.None);
			_mqttClient.MqttMsgPublishReceived += OnMsgReceived;
			_mqttClient.Connect(
				clientId: _mqttClientId,
				username: _mqttServerUserName,
				password: _mqttServerPassword,
				willRetain: true,
				willQosLevel: _qos,
				willFlag: true,
				willTopic: _statusTopic,
				willMessage: "Disconnected",
				cleanSession: true,
				keepAlivePeriod: 60);

			var subscriptionTopics = new String[]
			{
				_ledTopic, _pumpTopic
			};
			Subscribe(subscriptionTopics);

			_mqttClient.Publish(_statusTopic, "Connected", retain: true);

			_mqttClient.Publish(_ledTopic, "ON", _qos, retain: false);

			Thread.Sleep(Timeout.Infinite);

			//_mqttClient.Unsubscribe(subscriptionTopics);
			//_mqttClient.Publish(_statusTopic, "Disconnected", _qos, true);
			//_mqttClient.Disconnect();
		}
		private static void Subscribe(String[] topics)
		{
			foreach(var x in topics)
			{
				_mqttClient.Subscribe(new String[] { x }, new MqttQoSLevel[] { _qos });
			}
		}




		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private static void OnMsgReceived(Object sender, MqttMsgPublishEventArgs args)
		{
			var message = Encoding.UTF8.GetString(args.Message);
			Debug.WriteLine($"Publish Received Topic: {args.Topic}, Message: {message}");

			if(args.Topic.Equals(_pumpTopic))
			{
				_pumpPin.Write(PinValue.High);
				Thread.Sleep(_pumpActiveInterval);
				_pumpPin.Write(PinValue.Low);

				return;
			}

			if(args.Topic.Equals(_ledTopic))
			{
				_ledPin.Toggle();
				Thread.Sleep(125);
				_ledPin.Toggle();
				Thread.Sleep(125);
				_ledPin.Toggle();
				Thread.Sleep(125);
				_ledPin.Toggle();
				Thread.Sleep(525);

				return;
			}
		}

	}
}