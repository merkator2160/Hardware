using MqttMessageProcessor.Services.Interfaces;
using MqttMessageProcessor.Services.Models;
using MqttMessageProcessor.Services.Models.Config;
using MqttMessageProcessor.Services.Models.Enums;
using MqttMessageProcessor.Services.Models.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
			_mqttClient = new MqttClient(config.HostName, config.Port, false, null, null, MqttSslProtocols.None);
			_handlerDictionary = CreateHandlerDictionary();
		}


		// IProcessor /////////////////////////////////////////////////////////////////////////////
		public Boolean IsConnected => _mqttClient.IsConnected;

		public void Start()
		{
			if(_mqttClient.IsConnected)
				throw new MqttMessageProcessorException("Processor has already started!");

			_mqttClient.MqttMsgPublishReceived += OnMsgReceived;
			_mqttClient.Connect(_config.ClientId, _config.Login, _config.Password);

			SubscribeForTopics(_handlerDictionary.Keys.ToArray());
			Task.Run(StatusThread);
		}
		public void Stop()
		{
			_mqttClient?.Disconnect();
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private Dictionary<String, MqttClient.MqttMsgPublishEventHandler> CreateHandlerDictionary()
		{
			return new Dictionary<String, MqttClient.MqttMsgPublishEventHandler>()
			{
				{"domoticz/in", OnDomosticzInReceived},
				{"domoticz/out", OnDomosticzOutReceived},
			};
		}
		private void SubscribeForTopics(String[] topics)
		{
			foreach(var x in topics)
			{
				_mqttClient.Subscribe(new String[] { x }, new Byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
			}
		}
		private void Publish<T>(String topic, T obj, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
		{
			Publish(topic, JsonConvert.SerializeObject(obj));
		}
		private void Publish(String topic, String message, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
		{
			_mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
		}


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
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

			var climateSensorValues = domosticzInMessage.StringValue.Split(';');
			var temperature = climateSensorValues[0];
			var humidity = climateSensorValues[1];
			//var unknown = climateSensorValues[2];
			var pressureStr = climateSensorValues[3];
			//var unknown = climateSensorValues[4];

			var formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
			if(Single.TryParse(pressureStr, NumberStyles.AllowDecimalPoint, formatter, out var pressureGpa))
			{
				var pressureMpl = Math.Round(pressureGpa * 0.00750062, 2);    // 1 hectopascal [gPa] = 0,750063755419211 pressure in millimeters of mercury pillar (0°C) [mm mer.pill.]
				Publish("iotHub/goncharova/weather/pressure/mpl", pressureMpl);
			}

			Publish("iotHub/goncharova/weather/temperature", temperature);
			Publish("iotHub/goncharova/weather/humidity", humidity);
			Publish("iotHub/goncharova/weather/pressure/gpa", pressureStr);
		}
		private void OnDomosticzOutReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var domosticzOutMessage = JsonConvert.DeserializeObject<DomosticzOutMessage>(jsonMessage);

			if(domosticzOutMessage.DomosticzDeviceId != DomosticzDevice.ThermometerMyRoom)
				return;

			Publish("thermometerMiddleRoom/import/led", domosticzOutMessage.NumericValue);
		}


		// THREADS ////////////////////////////////////////////////////////////////////////////////
		private void StatusThread()
		{
			while(true)
			{
				if(_mqttClient.IsConnected)
					Publish("iotHub/status", "Connected");

				Thread.Sleep(10000);
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