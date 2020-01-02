using System;

namespace Common.Const.Units
{
	/// <summary>
	/// Structure representing temperature
	/// </summary>
	public struct Temperature
	{
		private const Double KelvinOffset = 273.15;
		private const Double FahrenheitOffset = 32.0;
		private const Double FahrenheitRatio = 1.8;
		private Double _celsius;

		private Temperature(Double celsius)
		{
			_celsius = celsius;
		}

		/// <summary>
		/// Temperature in Celsius
		/// </summary>
		public Double Celsius => _celsius;

		/// <summary>
		/// Temperature in Fahrenheit
		/// </summary>
		public Double Fahrenheit => FahrenheitRatio * _celsius + FahrenheitOffset;

		/// <summary>
		/// Temperature in Kelvin
		/// </summary>
		public Double Kelvin => _celsius + KelvinOffset;

		/// <summary>
		/// Creates Temperature instance from temperature in Celsius
		/// </summary>
		/// <param name="value">Temperature value in Celsius</param>
		/// <returns>Temperature instance</returns>
		public static Temperature FromCelsius(Double value)
		{
			return new Temperature(value);
		}

		/// <summary>
		/// Creates Temperature instance from temperature in Fahrenheit
		/// </summary>
		/// <param name="value">Temperature value in Fahrenheit</param>
		/// <returns>Temperature instance</returns>
		public static Temperature FromFahrenheit(Double value)
		{
			return new Temperature((value - FahrenheitOffset) / FahrenheitRatio);
		}

		/// <summary>
		/// Creates Temperature instance from temperature in Kelvin
		/// </summary>
		/// <param name="value">Temperature value in Kelvin</param>
		/// <returns>Temperature instance</returns>
		public static Temperature FromKelvin(Double value)
		{
			return new Temperature(value - KelvinOffset);
		}
	}
}