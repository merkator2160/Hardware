using Common.Drivers.Bmxx80;
using Common.Drivers.Bmxx80.PowerMode;
using System;
using System.Device.I2c;
using System.Threading;

namespace Raspberry.Sandbox.Samples
{
	internal sealed class Bme680Sample
	{
		public static void Main()
		{
			Console.WriteLine("Hello BME680!");

			// The I2C bus ID on the Raspberry Pi 3.
			const Int32 busId = 1;

			var i2cSettings = new I2cConnectionSettings(busId, Bme680Driver.DefaultI2cAddress);
			var i2cDevice = I2cDevice.Create(i2cSettings);

			using(var bme680 = new Bme680Driver(i2cDevice))
			{
				while(true)
				{
					// get the time a measurement will take with the current settings
					var measurementDuration = bme680.GetMeasurementDuration(bme680.HeaterProfile);

					// 10 consecutive measurement with default settings
					for(var i = 0; i < 10; i++)
					{
						// This instructs the sensor to take a measurement.
						bme680.SetPowerMode(Bme680PowerMode.Forced);

						// wait while measurement is being taken
						Thread.Sleep(measurementDuration);

						// Print out the measured data
						bme680.TryReadTemperature(out var temperature);
						bme680.TryReadPressure(out var pressure);
						bme680.TryReadHumidity(out var humidity);
						bme680.TryReadGasResistance(out var gasResistance);

						Console.WriteLine($"{temperature.Celsius:N2} \u00B0C | {pressure.Hectopascal:N2} hPa | {humidity:N2} %rH | {gasResistance:N2} Ohm");

						// when measuring the gas resistance on each cycle it is important to wait a certain interval
						// because a heating plate is activated which will heat up the sensor without sleep, this can
						// falsify all readings coming from the sensor
						Thread.Sleep(1000);
					}

					// change the settings
					bme680.TemperatureSampling = Sampling.HighResolution;
					bme680.HumiditySampling = Sampling.UltraHighResolution;
					bme680.PressureSampling = Sampling.Skipped;

					bme680.ConfigureHeatingProfile(Bme680HeaterProfile.Profile2, 280, 80, 24);
					bme680.HeaterProfile = Bme680HeaterProfile.Profile2;

					measurementDuration = bme680.GetMeasurementDuration(bme680.HeaterProfile);

					// 10 consecutive measurements with custom settings
					for(var i = 0; i < 10; i++)
					{
						// perform the measurement
						bme680.SetPowerMode(Bme680PowerMode.Forced);
						Thread.Sleep(measurementDuration);

						// Print out the measured data
						bme680.TryReadTemperature(out var temperature);
						bme680.TryReadPressure(out var pressure);
						bme680.TryReadHumidity(out var humidity);
						bme680.TryReadGasResistance(out var gasResistance);

						Console.WriteLine($"{temperature.Celsius:N2} \u00B0C | {pressure.Hectopascal:N2} hPa | {humidity:N2} %rH | {gasResistance:N2} Ohm");
						Thread.Sleep(1000);
					}

					// reset will change settings back to default
					bme680.Reset();
				}
			}
		}
	}
}