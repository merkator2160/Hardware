using CoordinateSharp;
using Hangfire;
using IotHub.Api.Services.Models.Config;
using IotHub.Common.Exceptions;
using IotHub.Common.Hangfire.Interfaces;
using ILogger = NLog.ILogger;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
    internal class SideRoomGreenhouseLightCelestialSchedulerJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;


        public SideRoomGreenhouseLightCelestialSchedulerJob(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        // IJob ///////////////////////////////////////////////////////////////////////////////////
        [AutomaticRetry(Attempts = 0)]
        public void Execute()
        {
            try
            {
                var locations = _configuration.GetSection("Locations").Get<LocationConfig[]>();
                var nizhniyNovgorod = locations[0];
                var cel = Celestial.CalculateCelestialTimes(nizhniyNovgorod.Latitude, nizhniyNovgorod.Longitude, DateTime.UtcNow);

                if (!cel.SunRise.HasValue)
                    throw new ValueNotFoundException($"{nameof(cel.SunRise)} has no value!");

                BackgroundJob.Schedule<SideRoomGreenhouseLightTurnOnJob>(p => p.Execute(), cel.SunRise.Value.ToLocalTime());

                if (!cel.SunSet.HasValue)
                    throw new ValueNotFoundException($"{nameof(cel.SunSet)} has no value!");

                BackgroundJob.Schedule<SideRoomGreenhouseLightTurnOffJob>(p => p.Execute(), cel.SunSet.Value.ToLocalTime());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{ex.Message}\r\n{ex.StackTrace}");
                throw;
            }
        }
    }
}