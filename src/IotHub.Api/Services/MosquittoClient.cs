﻿using IotHub.Api.Services.Interfaces;
using IotHub.Api.Services.Models.Config;
using IotHub.Api.Services.Models.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services
{
	internal partial class MosquittoClient : IMosquittoClient, IMqttPublisher, IDisposable
	{
		private readonly ProcessorConfig _config;
		private readonly Dictionary<String, MqttClient.MqttMsgPublishEventHandler> _handlerDictionary;
		private readonly MqttClient _mqttClient;

		private Boolean _disposed;


		public MosquittoClient(ProcessorConfig config)
		{
			_config = config;
			_mqttClient = new MqttClient(config.HostName, config.Port, false, null, null, MqttSslProtocols.None);
			_handlerDictionary = CreateHandlerDictionary();
		}


		// IMosquittoClient //////////////////////////////////////////////////////////////////
		public void Start()
		{
			if(_mqttClient.IsConnected)
				throw new MqttMessageProcessorException($"\"{nameof(MosquittoClient)}\" has already started!");

			_mqttClient.MqttMsgPublishReceived += OnMsgReceived;
			_mqttClient.Connect(_config.ClientId, _config.Login, _config.Password);

			SubscribeForTopics(_handlerDictionary.Keys.ToArray());

			Publish("iotHub/status", "Connected", MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
		}
		public void Stop()
		{
			Publish("iotHub/status", "Disconnected", MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);

			_mqttClient?.Disconnect();
		}


		// IMqttPublisher /////////////////////////////////////////////////////////////////////////
		public Boolean IsConnected => _mqttClient.IsConnected;

		public void Publish<T>(String topic, T obj, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
		{
			Publish(topic, JsonConvert.SerializeObject(obj));
		}
		public void Publish(String topic, String message, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
		{
			_mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnMsgReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			_handlerDictionary[eventArgs.Topic].Invoke(sender, eventArgs);
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		public Dictionary<String, MqttClient.MqttMsgPublishEventHandler> CreateHandlerDictionary()
		{
			var handlerDictionary = new Dictionary<String, MqttClient.MqttMsgPublishEventHandler>();

			AddDomosticzHandlers(handlerDictionary);

			return handlerDictionary;
		}
		private void SubscribeForTopics(String[] topics)
		{
			foreach(var x in topics)
			{
				_mqttClient.Subscribe(new String[] { x }, new Byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
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