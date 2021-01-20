using MqttMessageProcessor.Interfaces;
using MqttMessageProcessor.Models;
using MqttMessageProcessor.Models.Config;
using MqttMessageProcessor.Models.Enums;
using MqttMessageProcessor.Models.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttMessageProcessor
{
	internal class Processor : IProcessor
	{
		private readonly ProcessorConfig _config;
		private readonly Dictionary<String, MqttClient.MqttMsgPublishEventHandler> _handlerDictionary;
		private readonly MqttClient _mqttClient;

		private Boolean _disposed;


		public Processor(ProcessorConfig config)
		{
			_config = config;
			_mqttClient = new MqttClient(config.IpAddress);
			_handlerDictionary = CreateHandlerDictionary();
		}


		// IProcessor /////////////////////////////////////////////////////////////////////////////
		public Boolean IsConnected => _mqttClient.IsConnected;

		public void Start()
		{
			if(_mqttClient.IsConnected)
				throw new MqttMessageProcessorException("Processor has already started!");

			_mqttClient.MqttMsgPublishReceived += OnMsgReceived;
			_mqttClient.Connect(Guid.NewGuid().ToString());

			_mqttClient.Subscribe(_handlerDictionary.Keys.ToArray(), new Byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
		}
		public void Stop()
		{
			_mqttClient?.Disconnect();
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private void Publish<T>(String topic, T obj)
		{
			var json = JsonConvert.SerializeObject(obj);
			var data = Encoding.UTF8.GetBytes(json);

			_mqttClient.Publish("/test", data, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private Dictionary<String, MqttClient.MqttMsgPublishEventHandler> CreateHandlerDictionary()
		{
			return new Dictionary<String, MqttClient.MqttMsgPublishEventHandler>()
			{
				{"domoticz/in", OnDomosticzInReceived},
				{"domoticz/out", OnDomosticzOutReceived},
			};
		}
		private void OnMsgReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			_handlerDictionary[eventArgs.Topic].Invoke(sender, eventArgs);
		}
		private void OnDomosticzInReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var domosticzInMessage = JsonConvert.DeserializeObject<DomosticzInMessage>(jsonMessage);
			if(domosticzInMessage.DeviceId != DomosticzDevice.WeatherStation)
				return;

			var externalTemperature = domosticzInMessage.StringValue.Split(';')[0];

			_mqttClient.Publish("external/temperature", Encoding.UTF8.GetBytes(externalTemperature), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}
		private void OnDomosticzOutReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var domosticzOutMessage = JsonConvert.DeserializeObject<DomosticzOutMessage>(jsonMessage);
			if(domosticzOutMessage.DomosticzDeviceId != DomosticzDevice.ThermometerMyRoom)
				return;

			_mqttClient.Publish("thermometerMyRoom/import/led", Encoding.UTF8.GetBytes(domosticzOutMessage.NumericValue.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
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