using Nano.Common.Extensions;
using Nano.Sandbox.Models;
using nanoFramework.Json;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Nano.Sandbox.Units
{
	public class MqttUnit
	{
		private const String _brokerAddress = "192.168.1.11";
		private const String _mqttClientId = "588b1499-2f29-4c8a-82ef-178d26b56c5d";
		private const String _mqttServerUserName = "user";
		private const String _mqttServerPassword = ";4,wnmCvQ.7hMDdWuqv*";
		private const String _statusTopic = "esp32/status";
		private const MqttQoSLevel _qos = MqttQoSLevel.ExactlyOnce;

		private static MqttClient _mqttClient;


		public static void Run()
		{
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

			_mqttClient.Publish(_statusTopic, "Connected", retain: true);


			var test = JsonConvert.SerializeObject(new TestMsg
			{
				Message = "field"
			});
			Debug.WriteLine(test);
			_mqttClient.Publish("esp32/json", test, _qos, true);

			_mqttClient.Publish("esp32/json2", JsonConvert.SerializeObject(new TestMsg
			{
				Message = "field 2"
			}), _qos, true);

			//_mqttClient.Publish("esp32/json3", new TestMsg
			//{
			//	Message = "field 3"
			//}, _qos, true);


			var subscriptionTopics = new String[]
			{
				"test1", "test2"
			};
			_mqttClient.Subscribe(subscriptionTopics, new MqttQoSLevel[]
			{
				_qos, _qos
			});

			for(var i = 0; i < 10; i++)
			{
				var message = $"Test message {i}";
				Debug.WriteLine(message);
				_mqttClient.Publish("esp32/test", message, _qos);
				Thread.Sleep(1000);
			}

			//_mqttClient.Unsubscribe(subscriptionTopics);
			//Publish(_statusTopic, "Disconnected", _qos, true);
			//_mqttClient.Disconnect();
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private static void OnMsgReceived(Object sender, MqttMsgPublishEventArgs args)
		{
			var message = Encoding.UTF8.GetString(args.Message);

			Debug.WriteLine($"Publish Received Topic: {args.Topic} Message: {message}");
		}
	}
}