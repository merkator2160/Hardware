using Hangfire;
using IotHub.Api.Services.Interfaces;
using IotHub.Common.Hangfire.Interfaces;
using ILogger = NLog.ILogger;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
    internal class GreenhouseLightTurnOnJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IGreenhouseMqttLightControl _greenhouseMqttLightControl;


        public GreenhouseLightTurnOnJob(ILogger logger, IGreenhouseMqttLightControl greenhouseMqttLightControl)
        {
            _logger = logger;
            _greenhouseMqttLightControl = greenhouseMqttLightControl;
        }


        // IJob ///////////////////////////////////////////////////////////////////////////////////
        [AutomaticRetry(Attempts = 10)]
        public void Execute()
        {
            try
            {
                _greenhouseMqttLightControl.TurnOnSideRoomGreenhouseLight();
                _greenhouseMqttLightControl.TurnOnMiddleRoomGreenhouseLight();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{ex.Message}\r\n{ex.StackTrace}");
                throw;
            }
        }
    }
}