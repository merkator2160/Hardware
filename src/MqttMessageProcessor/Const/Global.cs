using System;

namespace MqttMessageProcessor.Const
{
	public static class Global
	{
		public const Double PressureCoefficient = 0.750063755419211;    // 1 hectopascal [gPa] = 0,750063755419211 pressure in millimeters of mercury pillar (0°C) [mm mer.pill.]
		public const Single SeaLevelPressureHpa = 1013.25F;
	}
}