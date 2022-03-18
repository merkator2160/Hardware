using AutoMapper;
using IotHub.Api.Services.Models;
using IotHub.Contracts.Models.Api.DeviceMonitor;

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