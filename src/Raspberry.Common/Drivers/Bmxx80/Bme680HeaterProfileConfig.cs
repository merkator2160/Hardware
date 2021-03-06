﻿using System;

namespace Common.Drivers.Bmxx80
{
	/// <summary>
	/// The heater profile configuration saved on the device.
	/// </summary>
	public class Bme680HeaterProfileConfig
	{
		/// <summary>
		/// The chosen heater profile slot, ranging from 0-9.
		/// </summary>
		public Bme680HeaterProfile HeaterProfile { get; set; }

		/// <summary>
		/// The heater resistance.
		/// </summary>
		public UInt16 HeaterResistance { get; set; }

		/// <summary>
		/// The heater duration in the internally used format.
		/// </summary>
		public UInt16 HeaterDuration { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="Bme680HeaterProfileConfig"/>.
		/// </summary>
		/// <param name="profile">The used heater profile.</param>
		/// <param name="heaterResistance">The heater resistance in Ohm.</param>
		/// <param name="heaterDuration">The heating duration in ms.</param>
		/// <exception cref="ArgumentOutOfRangeException">Unknown profile setting used</exception>
		public Bme680HeaterProfileConfig(Bme680HeaterProfile profile, UInt16 heaterResistance, UInt16 heaterDuration)
		{
			if(!Enum.IsDefined(typeof(Bme680HeaterProfile), profile))
			{
				throw new ArgumentOutOfRangeException();
			}

			HeaterProfile = profile;
			HeaterResistance = heaterResistance;
			HeaterDuration = heaterDuration;
		}
	}
}
