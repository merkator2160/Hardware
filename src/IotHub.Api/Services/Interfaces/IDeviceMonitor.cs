using IotHub.Api.Services.Models;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Interfaces
{
    public interface IDeviceMonitor
    {
        TrackingDeviceDto[] GetAllDeviceInfo();
        TrackingDeviceDto[] GetAllUnavailableDeviceInfo();
        void UpdateDeviceState(MqttMsgPublishEventArgs args);
        String[] GetSubscriptionTopics();
    }
}