using System;

namespace IotHub.Api.Services.Interfaces
{
	internal interface ISideRoomMqttLightControl
	{
		Boolean IsConnected { get; }

		void ToggleSideRoomGreenhouseLight();
		void TurnOnSideRoomGreenhouseLight();
		void TurnOffSideRoomGreenhouseLight();
	}
}