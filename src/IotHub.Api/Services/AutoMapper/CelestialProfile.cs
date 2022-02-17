using AutoMapper;
using IotHub.Api.Services.Models.Config;
using IotHub.UnitTests;

namespace IotHub.Api.Services.AutoMapper
{
    internal class CelestialProfile : Profile
    {
        public CelestialProfile()
        {
            CreateMap<LocationConfig, LocationAm>();
        }
    }
}