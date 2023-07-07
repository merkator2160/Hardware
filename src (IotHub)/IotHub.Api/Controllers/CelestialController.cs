using AutoMapper;
using Common.Contracts.Api.Celestial;
using Common.Contracts.Exceptions.Application;
using CoordinateSharp;
using IotHub.Api.Services.Models.Config;
using Microsoft.AspNetCore.Mvc;

namespace IotHub.Api.Controllers
{
    // Reference: https://world-weather.ru/pogoda/russia/nizhny_novgorod/sunrise/
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CelestialController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public CelestialController(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }


        // ACTIONS //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns celestial information for locations predefined in the configuration
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(LocationCelestialInfoAm[]), 200)]
        [ProducesResponseType(typeof(String), 460)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult GetCelestialInfoForConfigLocations()
        {
            var locations = _configuration.GetSection("Locations").Get<LocationConfig[]>();
            var locationInfoList = new List<LocationCelestialInfoAm>(locations.Length);
            var dateTimeUtc = DateTime.UtcNow;

            foreach (var x in locations)
            {
                var cel = Celestial.CalculateCelestialTimes(x.Latitude, x.Longitude, dateTimeUtc);

                if (!cel.SunRise.HasValue)
                    throw new ValueNotFoundException($"{nameof(cel.SunRise)} has no value!");

                if (!cel.SunSet.HasValue)
                    throw new ValueNotFoundException($"{nameof(cel.SunSet)} has no value!");

                locationInfoList.Add(new LocationCelestialInfoAm()
                {
                    SunRise = cel.SunRise.Value.ToLocalTime(),
                    SunSet = cel.SunSet.Value.ToLocalTime(),
                    Location = _mapper.Map<LocationAm>(x)
                });
            }

            return Ok(locationInfoList.ToArray());
        }

        /// <summary>
        /// Returns celestial information for provided location
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(LocationCelestialInfoAm), 200)]
        [ProducesResponseType(typeof(String), 460)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult GetCelestialInfoForLocation(Single latitude, Single longitude)
        {
            var cel = Celestial.CalculateCelestialTimes(latitude, longitude, DateTime.UtcNow);

            if (!cel.SunRise.HasValue)
                throw new ValueNotFoundException($"{nameof(cel.SunRise)} has no value!");

            if (!cel.SunSet.HasValue)
                throw new ValueNotFoundException($"{nameof(cel.SunSet)} has no value!");

            return Ok(new LocationCelestialInfoAm()
            {
                SunRise = cel.SunRise.Value.ToLocalTime(),
                SunSet = cel.SunSet.Value.ToLocalTime(),
                Location = new LocationAm()
                {
                    Name = "N/A",
                    Latitude = latitude,
                    Longitude = longitude
                }
            });
        }
    }
}