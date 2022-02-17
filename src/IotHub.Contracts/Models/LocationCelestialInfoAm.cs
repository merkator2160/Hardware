using IotHub.UnitTests;
using System;

namespace IotHub.Contracts.Models
{
    public class LocationCelestialInfoAm
    {
        public DateTime SunRise { get; set; }
        public DateTime SunSet { get; set; }
        public LocationAm Location { get; set; }
    }
}