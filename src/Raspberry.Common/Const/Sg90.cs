﻿using Common.Drivers;
using Common.Drivers.Pca9685;
using System;

namespace Common.Const
{
	public static class Sg90
	{
		public const Double MinPositionFrq = 0.025;
		public const Double MaxPositionFrq = 0.108;

		public const Int32 MaxAngle = 180;
		public const Int32 MinPulseWidthMicroseconds = 600;
		public const Int32 MaxPulseWidthMicroseconds = 2600;


		public static ServoMotor CreateSg90Servo(this Pca9685Driver pca9685, Int32 channel)
		{
			return new ServoMotor(pca9685.CreatePwmChannel(channel), MaxAngle, MinPulseWidthMicroseconds, MaxPulseWidthMicroseconds);
		}
	}
}