using System;
using System.Device.Pwm;

namespace Common.Drivers.Pca9685.Interfaces
{
	public interface IPca9685
	{
		Double ClockFrequency { get; set; }
		Double PwmFrequency { get; set; }

		void SetDutyCycle(Int32 channel, Double dutyCycle);
		Double GetDutyCycle(Int32 channel);
		void SetDutyCycleAllChannels(Double dutyCycle);
		PwmChannel CreatePwmChannel(Int32 channel);
	}
}