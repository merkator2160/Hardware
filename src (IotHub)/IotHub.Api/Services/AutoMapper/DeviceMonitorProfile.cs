using AutoMapper;
using Common.Contracts.Api.DeviceMonitor;
using IotHub.Api.Services.Models;

namespace IotHub.Api.Services.AutoMapper
{
    internal class DeviceMonitorProfile : Profile
    {
        public DeviceMonitorProfile()
        {
            CreateMap<TrackingDeviceDto, DeviceUnderTrackingAm>();
        }
    }
}