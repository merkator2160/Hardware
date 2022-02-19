using System;

namespace IotHub.Contracts.Models.Api.Celestial
{
    public class LocationCelestialInfoAm
    {
        public DateTime SunRise { get; set; }
        public DateTime SunSet { get; set; }
        public LocationAm Location { get; set; }
    }
}