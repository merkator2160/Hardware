using Common.Const.Units;
using Common.Drivers.Bmxx80;
using Common.Drivers.Bmxx80.FilteringMode;
using Common.Drivers.Bmxx80.PowerMode;
using System;
using System.Device.I2c;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Bmp280Unit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			var defaultSeaLevelPressure = Pressure.MeanSeaLevel;

			var i2cDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x76));
			var i2CBmp280 = new Bme280Driver(i2cDevice);
			using(i2CBmp280)
			{
				while(true)
				{
					i2CBmp280.TemperatureSampling = Sampling.LowPower;
					i2CBmp280.PressureSampling = Sampling.UltraHighResolution;
					i2CBmp280.HumiditySampling = Sampling.Standard;

					// set mode forced so device sleeps after read
					i2CBmp280.SetPowerMode(Bmx280PowerMode.Forced);

					// wait for measurement to be performed
					var measurementTime = i2CBmp280.GetMeasurementDuration();
					Thread.Sleep(measurementTime);

					// read values
					i2CBmp280.TryReadTemperature(out var tempValue);
					Console.WriteLine($"Temperature: {tempValue.Celsius} \u00B0C");
					i2CBmp280.TryReadPressure(out var preValue);
					Console.WriteLine($"Pressure: {preValue.Hectopascal} hPa");
					i2CBmp280.TryReadAltitude(defaultSeaLevelPressure, out var altValue);
					Console.WriteLine($"Altitude: {altValue} meters");
					i2CBmp280.TryReadHumidity(out var humValue);
					Console.WriteLine($"Humidity: {humValue} %");
					Thread.Sleep(1000);

					// change sampling and filter
					i2CBmp280.TemperatureSampling = Sampling.UltraHighResolution;
					i2CBmp280.PressureSampling = Sampling.UltraLowPower;
					i2CBmp280.HumiditySampling = Sampling.UltraLowPower;
					i2CBmp280.FilterMode = Bmx280FilteringMode.X2;

					// set mode forced and read again
					i2CBmp280.SetPowerMode(Bmx280PowerMode.Forced);

					// wait for measurement to be performed
					measurementTime = i2CBmp280.GetMeasurementDuration();
					Thread.Sleep(measurementTime);

					// read values
					i2CBmp280.TryReadTemperature(out tempValue);
					Console.WriteLine($"Temperature: {tempValue.Celsius} \u00B0C");
					i2CBmp280.TryReadPressure(out preValue);
					Console.WriteLine($"Pressure: {preValue.Hectopascal} hPa");
					i2CBmp280.TryReadAltitude(defaultSeaLevelPressure, out altValue);
					Console.WriteLine($"Altitude: {altValue} meters");
					i2CBmp280.TryReadHumidity(out humValue);
					Console.WriteLine($"Humidity: {humValue} %");
					Thread.Sleep(5000);
				}
			}
		}
	}
}