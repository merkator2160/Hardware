using IotHub.Api.Services.Interfaces;
using IotHub.Api.Services.Models;
using IotHub.Api.Services.Models.Config;
using IotHub.Api.Services.Models.Enums;
using IotHub.Api.Services.Models.Exceptions;
using IotHub.Common.Const;
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

namespace IotHub.Api.Services
{
	internal class MqttMessageProcessor : IMqttMessageProcessor, IMqttPublisher, IDisposable
	{
		private readonly ProcessorConfig _config;
		private readonly Dictionary<String, MqttClient.MqttMsgPublishEventHandler> _handlerDictionary;
		private readonly MqttClient _mqttClient;

		private Boolean _disposed;


		public MqttMessageProcessor(ProcessorConfig config)
		{
			_config = config;
			_mqttClient = new MqttClient(config.HostName, config.Port, false, null, null, MqttSslProtocols.None);
			_handlerDictionary = CreateHandlerDictionary();
		}


		// IMqttMessageProcessor //////////////////////////////////////////////////////////////////
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


		// IMqttPublisher /////////////////////////////////////////////////////////////////////////
		public void Publish<T>(String topic, T obj, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
		{
			Publish(topic, JsonConvert.SerializeObject(obj));
		}
		public void Publish(String topic, String message, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
		{
			_mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
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


		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnMsgReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			_handlerDictionary[eventArgs.Topic].Invoke(sender, eventArgs);
		}
		private void OnDomosticzInReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<DomosticzInMessage>(jsonMessage);

			if(message.DeviceId != DomosticzDevice.WeatherStation)
				HandleWeatherStationMessage(message);
		}
		private void HandleWeatherStationMessage(DomosticzInMessage message)
		{
			var climateSensorValues = message.StringValue.Split(';');
			var temperature = climateSensorValues[0];
			var humidity = climateSensorValues[1];
			//var unknown = climateSensorValues[2];
			var pressureStr = climateSensorValues[3];
			//var unknown = climateSensorValues[4];

			var formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
			if(Single.TryParse(pressureStr, NumberStyles.AllowDecimalPoint, formatter, out var pressureGpa))
			{
				var pressureMpl = Math.Round(pressureGpa * Global.PressureCoefficient, 2);
				Publish("iotHub/goncharova/weather/pressure/mpl", pressureMpl);
			}

			Publish("iotHub/goncharova/weather/temperature", temperature);
			Publish("iotHub/goncharova/weather/humidity", humidity);
			Publish("iotHub/goncharova/weather/pressure/gpa", pressureStr);
		}

		private void OnDomosticzOutReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
		{
			var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
			var message = JsonConvert.DeserializeObject<DomosticzOutMessage>(jsonMessage);

			if(message.DomosticzDeviceId == DomosticzDevice.ThermometerMyRoom)
				HandleThermometerMyRoomMessage(message);
		}
		private void HandleThermometerMyRoomMessage(DomosticzOutMessage message)
		{
			Publish("thermometerMiddleRoom/import/led", message.NumericValue);
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