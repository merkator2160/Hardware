using System;

namespace IotHub.Contracts.Models.Api.Other
{
    public class WeatherForecastAm
    {
        public DateTime Date { get; set; }
        public Int32 TemperatureC { get; set; }
        public String? Summary { get; set; }
        public Int32 TemperatureF => 32 + (Int32)(TemperatureC / 0.5556);
    }
}