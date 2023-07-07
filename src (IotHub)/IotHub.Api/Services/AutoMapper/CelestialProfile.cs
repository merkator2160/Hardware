using AutoMapper;
using Common.Contracts.Api.Celestial;
using IotHub.Api.Services.Models.Config;

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