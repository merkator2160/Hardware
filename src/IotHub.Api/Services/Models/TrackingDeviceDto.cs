using System;

namespace IotHub.Api.Services.Models
{
    public class TrackingDeviceDto
    {
        public String Name { get; set; }
        public String AvailabilityTopic { get; set; }
        public DateTime LastAvailable { get; set; }
    }
}