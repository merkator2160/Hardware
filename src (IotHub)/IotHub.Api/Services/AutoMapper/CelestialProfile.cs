using AutoMapper;
using IotHub.Api.Services.Models.Config;
using IotHub.Contracts.Models.Api.Celestial;

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