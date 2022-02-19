using System;

namespace IotHub.Api.Services.Models.Config
{
    internal class DeviceMonitorConfig
    {
        public Byte DevicePingTimeOutMin { get; set; }
        public TrackingDeviceConfig[] Devices { get; set; }
    }
}