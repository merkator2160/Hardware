using Hangfire;
using Hangfire.Interfaces;
using IotHub.Api.Services.Interfaces;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
    internal class GreenhouseLightTurnOnJob : IJob
    {
        private readonly IGreenhouseMqttLightControl _greenhouseMqttLightControl;


        public GreenhouseLightTurnOnJob(IGreenhouseMqttLightControl greenhouseMqttLightControl)
        {
            _greenhouseMqttLightControl = greenhouseMqttLightControl;
        }


        // IJob ///////////////////////////////////////////////////////////////////////////////////
        [AutomaticRetry(Attempts = 10)]
        public void Execute()
        {
            _greenhouseMqttLightControl.TurnOnSideRoomGreenhouseLight();
            _greenhouseMqttLightControl.TurnOnMiddleRoomGreenhouseLight();
        }
    }
}