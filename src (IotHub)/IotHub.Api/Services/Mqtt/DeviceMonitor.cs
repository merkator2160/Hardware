using IotHub.Api.Services.Interfaces;
using IotHub.Api.Services.Models;
using IotHub.Api.Services.Models.Config;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Mqtt
{
    internal class DeviceMonitor : IDeviceMonitor
    {
        private readonly DeviceMonitorConfig _config;
        private readonly Dictionary<String, TrackingDeviceDto> _deviceDictionary;


        public DeviceMonitor(DeviceMonitorConfig config)
        {
            _config = config;
            _deviceDictionary = CreateDeviceDictionary(config.Devices);
        }


        // IDeviceMonitor /////////////////////////////////////////////////////////////////////////
        public TrackingDeviceDto[] GetAllDeviceInfo()
        {
            lock (_deviceDictionary)
            {
                return _deviceDictionary.Values.ToArray();
            }
        }
        public TrackingDeviceDto[] GetAllUnavailableDeviceInfo()
        {
            lock (_deviceDictionary)
            {
                var threshold = DateTime.Now.AddMinutes(-_config.DevicePingTimeOutMin);

                return _deviceDictionary.Values.Where(p => p.LastAvailable < threshold).ToArray();
            }
        }
        public void UpdateDeviceState(MqttMsgPublishEventArgs args)
        {
            lock (_deviceDictionary)
            {
                if (!_deviceDictionary.ContainsKey(args.Topic))
                    return;

                _deviceDictionary[args.Topic].LastAvailable = DateTime.Now;
            }
        }
        public String[] GetSubscriptionTopics()
        {
            lock (_deviceDictionary)
            {
                return _deviceDictionary.Keys.ToArray();
            }
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private Dictionary<String, TrackingDeviceDto> CreateDeviceDictionary(TrackingDeviceConfig[] devices)
        {
            return devices.ToDictionary(x => x.AvailabilityTopic, x => new TrackingDeviceDto()
            {
                AvailabilityTopic = x.AvailabilityTopic,
                Name = x.Name,
                LastAvailable = DateTime.Now
            });
        }
    }
}