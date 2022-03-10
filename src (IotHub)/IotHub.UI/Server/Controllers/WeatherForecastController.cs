using System;
using System.Collections.Generic;
using System.Linq;
using IotHub.Contracts.Models.Api.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IotHub.UI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private static readonly String[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IEnumerable<WeatherForecastAm> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastAm
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = _summaries[Random.Shared.Next(_summaries.Length)]
            })
            .ToArray();
        }
    }
}