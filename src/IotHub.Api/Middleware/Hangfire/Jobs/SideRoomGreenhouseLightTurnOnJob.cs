using Hangfire;
using IotHub.Api.Services.Interfaces;
using IotHub.Common.Hangfire.Interfaces;
using NLog;
using System;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
    internal class SideRoomGreenhouseLightTurnOnJob : IJob
    {
        private readonly ILogger _logger;
        private readonly ISideRoomMqttLightControl _sideRoomMqttLightControl;


        public SideRoomGreenhouseLightTurnOnJob(ILogger logger, ISideRoomMqttLightControl sideRoomMqttLightControl)
        {
            _logger = logger;
            _sideRoomMqttLightControl = sideRoomMqttLightControl;
        }


        // IJob ///////////////////////////////////////////////////////////////////////////////////
        [AutomaticRetry(Attempts = 10)]
        public void Execute()
        {
            try
            {
                _sideRoomMqttLightControl.TurnOnSideRoomGreenhouseLight();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{ex.Message}\r\n{ex.StackTrace}");
                throw;
            }
        }
    }
}