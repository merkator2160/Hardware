using MqttMessageProcessor.Interfaces;
using MqttMessageProcessor.Models;
using Newtonsoft.Json;
using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttMessageProcessor
{
	internal class Processor : IProcessor
	{
		private readonly String _brokerIpAddress;

		private MqttClient _mqttClient;
		private Boolean _disposed;


		public Processor(String brokerIpAddress)
		{
			_brokerIpAddress = brokerIpAddress;
		}


		// IProcessor /////////////////////////////////////////////////////////////////////////////
		public void Start()
		{
			if(_mqttClient != null)
				throw new ApplicationException("Processor has already started!");

			_mqttClient = new MqttClient(_brokerIpAddress);
			_mqttClient.MqttMsgPublishReceived += OnMqttMsgPublishReceived;

			_mqttClient.Connect(Guid.NewGuid().ToString());

			_mqttClient.Subscribe(new String[] { "domoticz/in" }, new Byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
			_mqttClient.Subscribe(new String[] { "domoticz/out" }, new Byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		}
		public void Stop()
		{
			_mqttClient?.Disconnect();
			_mqttClient = null;
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private void Publish<T>(String topic, T obj)
		{
			var json = JsonConvert.SerializeObject(obj);
			var data = Encoding.UTF8.GetBytes(json);

			_mqttClient.Publish("/test", data, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnMqttMsgPublishReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);

			if(eventArgs.Topic.Equals("domoticz/in"))
			{
				var domosticzInMessage = JsonConvert.DeserializeObject<DomosticzInMessage>(jsonMessage);
				if(domosticzInMessage.DeviceId == 1)
				{
					var externalTemperature = domosticzInMessage.Svalue.Split(';')[0];

					_mqttClient.Publish("external/temperature", Encoding.UTF8.GetBytes(externalTemperature), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
				}
			}

			if(eventArgs.Topic.Equals("domoticz/out"))
			{
				var domosticzInMessage = JsonConvert.DeserializeObject<DomosticzOutMessage>(jsonMessage);
				if(domosticzInMessage.DeviceId == 3)
				{
					_mqttClient.Publish("thermometerMyRoom/import/led", Encoding.UTF8.GetBytes(domosticzInMessage.Nvalue.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
				}
			}
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		public void Dispose()
		{
			if(_disposed)
				return;

			Stop();

			_disposed = true;
		}
	}
}