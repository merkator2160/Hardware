using Common.Contracts.Exceptions.Application;
using CoordinateSharp;
using Hangfire;
using Hangfire.Interfaces;
using IotHub.Api.Services.Models.Config;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
    internal class GreenhouseLightCelestialSchedulerJob : IJob
    {
        private readonly IConfiguration _configuration;


        public GreenhouseLightCelestialSchedulerJob(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // IJob ///////////////////////////////////////////////////////////////////////////////////
        [AutomaticRetry(Attempts = 0)]
        public void Execute()
        {
            var locations = _configuration.GetSection("Locations").Get<LocationConfig[]>();
            var nizhniyNovgorod = locations[0];
            var cel = Celestial.CalculateCelestialTimes(nizhniyNovgorod.Latitude, nizhniyNovgorod.Longitude, DateTime.UtcNow);

            if (!cel.SunRise.HasValue)
                throw new ValueNotFoundException($"{nameof(cel.SunRise)} has no value!");

            BackgroundJob.Schedule<GreenhouseLightTurnOnJob>(p => p.Execute(), cel.SunRise.Value.ToLocalTime());

            if (!cel.SunSet.HasValue)
                throw new ValueNotFoundException($"{nameof(cel.SunSet)} has no value!");

            BackgroundJob.Schedule<GreenhouseLightTurnOffJob>(p => p.Execute(), cel.SunSet.Value.ToLocalTime());
        }
    }
}