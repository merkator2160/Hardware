using Hangfire;
using Hangfire.Interfaces;
using IotHub.Api.Services.Interfaces;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
    internal class GreenhouseLightTurnOffJob : IJob
    {
        private readonly IGreenhouseMqttLightControl _greenhouseMqttLightControl;


        public GreenhouseLightTurnOffJob(IGreenhouseMqttLightControl greenhouseMqttLightControl)
        {
            _greenhouseMqttLightControl = greenhouseMqttLightControl;
        }


        // IJob ///////////////////////////////////////////////////////////////////////////////////
        [AutomaticRetry(Attempts = 10)]
        public void Execute()
        {
            _greenhouseMqttLightControl.TurnOffSideRoomGreenhouseLight();
            _greenhouseMqttLightControl.TurnOffMiddleRoomGreenhouseLight();
        }
    }
}