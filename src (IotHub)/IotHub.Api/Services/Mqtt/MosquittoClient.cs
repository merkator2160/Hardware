﻿using ApiClients.Http.EasyEsp;
using IotHub.Api.Services.Interfaces;
using IotHub.Api.Services.Models.Config;
using IotHub.Api.Services.Models.Exceptions;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Mqtt
{
    /// <summary>
    /// MQTT documentation: https://mosquitto.org/man/mqtt-7.html
    /// </summary>
    internal partial class MosquittoClient : IMosquittoClient, IMqttPublisher, IDisposable
    {
        private readonly ProcessorConfig _config;
        private readonly EasyEspClient _easyEspClient;
        private readonly IDeviceMonitor _deviceMonitor;
        private readonly Dictionary<String, MqttClient.MqttMsgPublishEventHandler> _handlerDictionary;
        private readonly MqttClient _mqttClient;
        private readonly JsonSerializerSettings _serializerSettings;

        private const String _statusTopic = "iotHub/status";

        private Boolean _disposed;


        public MosquittoClient(ProcessorConfig config, EasyEspClient easyEspClient, IDeviceMonitor deviceMonitor)
        {
            _config = config;
            _easyEspClient = easyEspClient;
            _deviceMonitor = deviceMonitor;
            _handlerDictionary = CreateHandlerDictionary();
            _mqttClient = new MqttClient(config.HostName, config.Port, false, null, null, MqttSslProtocols.None);
            _serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }


        // IMosquittoClient //////////////////////////////////////////////////////////////////
        public void Start()
        {
            if (_mqttClient.IsConnected)
                throw new MqttMessageProcessorException($"\"{nameof(MosquittoClient)}\" has already started!");

            _mqttClient.MqttMsgPublishReceived += OnMsgReceived;
            _mqttClient.Connect(
                clientId: _config.ClientId,
                username: _config.Login,
                password: _config.Password,
                willRetain: true,
                willQosLevel: MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                willFlag: true,
                willTopic: _statusTopic,
                willMessage: "Disconnected",
                cleanSession: true,
                keepAlivePeriod: 60);

            SubscribeForTopics(_handlerDictionary.Keys.ToArray());
            Publish(_statusTopic, "Connected", retain: true);
        }
        public void Stop()
        {
            if (!_mqttClient.IsConnected)
                return;

            Publish(_statusTopic, "Disconnected", retain: true);
            _mqttClient.Disconnect();
        }


        // IMqttPublisher /////////////////////////////////////////////////////////////////////////
        public Boolean IsConnected => _mqttClient.IsConnected;

        public void Publish<T>(String topic, T obj, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
        {
            Publish(topic, JsonConvert.SerializeObject(obj, _serializerSettings), qos, retain);
        }
        public void Publish(String topic, String message, Byte qos = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, Boolean retain = false)
        {
            _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void OnMsgReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            try
            {
                _deviceMonitor.UpdateDeviceState(eventArgs);

                if (!_handlerDictionary.ContainsKey(eventArgs.Topic))
                    return;

                _handlerDictionary[eventArgs.Topic].Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                Publish("iotHub/error", $"{ex.Message}\r\n{ex.StackTrace}", MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
            }
        }


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
        public Dictionary<String, MqttClient.MqttMsgPublishEventHandler> CreateHandlerDictionary()
        {
            var handlerDictionary = new Dictionary<String, MqttClient.MqttMsgPublishEventHandler>();
#if DEBUG
            AddDebugHandlers(handlerDictionary);
            AddMonitorLedHandlers(handlerDictionary);
            AddButtonHandlers(handlerDictionary);
            AddMiddleRoomHandlers(handlerDictionary);
#else
            AddMiddleRoomHandlers(handlerDictionary);
			AddDebugHandlers(handlerDictionary);
			AddButtonHandlers(handlerDictionary);
			AddSideRoomHandlers(handlerDictionary);
			AddWeatherHandlers(handlerDictionary);
			AddMonitorLedHandlers(handlerDictionary);
			AddIrHandlers(handlerDictionary);
			AddKitchenHandlers(handlerDictionary);
#endif
            return handlerDictionary;
        }
        private void SubscribeForTopics(String[] topics)
        {
            var monitoringTopics = _deviceMonitor.GetSubscriptionTopics();
            foreach (var x in monitoringTopics)
            {
                _mqttClient.Subscribe(new String[] { x }, new Byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            }

            foreach (var x in topics)
            {
                _mqttClient.Subscribe(new String[] { x }, new Byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            }
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (_disposed)
                return;

            Stop();

            _disposed = true;
        }
    }
}