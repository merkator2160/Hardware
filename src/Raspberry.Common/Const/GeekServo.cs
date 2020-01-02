using Common.Drivers;
using Common.Drivers.Pca9685;
using System;

namespace Common.Const
{
	public static class GeekServo
	{
		public const Double MinPositionFrq = 0.025;
		public const Double MaxPositionFrq = 0.108;

		public const Int32 MaxAngleRange = 270;
		public const Int32 MinPulseWidthMicroseconds = 600;
		public const Int32 MaxPulseWidthMicroseconds = 2600;


		public static ServoMotor CreateGeekServo(this Pca9685Driver pca9685, Int32 channel)
		{
			return new ServoMotor(pca9685.CreatePwmChannel(channel), MaxAngleRange, MinPulseWidthMicroseconds, MaxPulseWidthMicroseconds);
		}
	}
}