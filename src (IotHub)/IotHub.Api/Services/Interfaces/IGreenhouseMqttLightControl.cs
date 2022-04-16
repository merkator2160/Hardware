namespace IotHub.Api.Services.Interfaces
{
    internal interface IGreenhouseMqttLightControl
    {
        Boolean IsConnected { get; }

        // Side room //////////////////////////////////////////////////////////////////////////////
        void ToggleSideRoomGreenhouseLight();
        void TurnOnSideRoomGreenhouseLight();
        void TurnOffSideRoomGreenhouseLight();

        // Middle room ////////////////////////////////////////////////////////////////////////////
        void ToggleMiddleRoomGreenhouseLight();
        void TurnOnMiddleRoomGreenhouseLight();
        void TurnOffMiddleRoomGreenhouseLight();
    }
}