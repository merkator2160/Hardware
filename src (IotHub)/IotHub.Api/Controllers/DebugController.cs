﻿using ApiClients.Http.EasyEsp;
using AutoMapper;
using Common.Contracts.Api.Other;
using Common.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace IotHub.Api.Controllers
{
    /// <summary>
    /// This controller only for debugging, playing, testing purposes (it does not appear in production)
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DebugController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DebugController> _logger;
        private readonly IMapper _mapper;
        private readonly EasyEspHttpClient _easyEspClient;


        public DebugController(
            IWebHostEnvironment env,
            ILogger<DebugController> logger,
            IMapper mapper,
            EasyEspHttpClient easyEspClient)
        {
            _env = env;
            _logger = logger;
            _mapper = mapper;
            _easyEspClient = easyEspClient;
        }


        // ACTIONS //////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creates a log entry
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult WriteLog(String message)
        {
            _logger.LogError($"Test error log: {message}");
            _logger.LogDebug($"Test debug log: {message}");

            return Ok();
        }

        /// <summary>
        /// Returns current environment name retrieved from IWebHostEnvironment
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult GetAspNetEnvironment()
        {
            return Ok(_env.EnvironmentName);
        }

        /// <summary>
        /// Returns a raw current server environment variable value introduced in current server global environment variables pool
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(String), 200)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult GetRawEnvironmentVariable()
        {
            return Ok(Environment.GetEnvironmentVariable(CustomConfigurationProvider.DefaultEnvironmentVariableName));
        }

        /// <summary>
        /// Returns name of the variable which uses to determine current server environment
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(String), 200)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult GetRawEnvironmentVariableName()
        {
            return Ok(CustomConfigurationProvider.DefaultEnvironmentVariableName);
        }

        /// <summary>
        /// Returns external internet IP address
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(String), 200)]
        [ProducesResponseType(typeof(String), 500)]
        public async Task<IActionResult> GetExternalIpAddress()
        {
            using (var client = new HttpClient())
            {
                return Ok(await client.GetStringAsync("http://checkip.amazonaws.com/"));
            }
        }

        /// <summary>
        /// Plays sound on EastESP Unit2
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(String), 200)]
        [ProducesResponseType(typeof(String), 500)]
        public async Task<IActionResult> PlaySoundOnUnit2(String rtttl = "d=10,o=6,b=180,c,e,g")
        {
            await _easyEspClient.Unit2PlaySoundAsync(rtttl);

            return Ok();
        }

        /// <summary>
        /// Returns artificially generated weather forecast sample
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(String), 200)]
        [ProducesResponseType(typeof(String), 500)]
        public IEnumerable<WeatherForecastAm> GetWeatherForecast()
        {
            var random = new Random();
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            return Enumerable.Range(1, 5).Select(index => new WeatherForecastAm
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = random.Next(-20, 55),
                Summary = summaries[random.Next(summaries.Length)]
            }).ToArray();
        }
    }
}