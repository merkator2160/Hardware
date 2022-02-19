using Common.Const.Units;
using Common.Drivers.Bmxx80;
using Common.Drivers.Bmxx80.FilteringMode;
using Common.Drivers.Bmxx80.PowerMode;
using System;
using System.Device.I2c;
using System.Threading;

namespace Raspberry.Sandbox.Samples
{
    internal sealed class Bmp280Sample
    {
        public static void Main()
        {
            Console.WriteLine("Hello Bmp280!");

            // bus id on the raspberry pi 3
            const Int32 busId = 1;
            // set this to the current sea level pressure in the area for correct altitude readings
            var defaultSeaLevelPressure = Pressure.MeanSeaLevel;

            var i2cSettings = new I2cConnectionSettings(busId, Bmx280Base.DefaultI2cAddress);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            var i2CBmp280 = new Bmp280Driver(i2cDevice);

            using (i2CBmp280)
            {
                while (true)
                {
                    // set higher sampling
                    i2CBmp280.TemperatureSampling = Sampling.LowPower;
                    i2CBmp280.PressureSampling = Sampling.UltraHighResolution;

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
                    Console.WriteLine($"Altitude: {altValue} m");
                    Thread.Sleep(1000);

                    // change sampling rate
                    i2CBmp280.TemperatureSampling = Sampling.UltraHighResolution;
                    i2CBmp280.PressureSampling = Sampling.UltraLowPower;
                    i2CBmp280.FilterMode = Bmx280FilteringMode.X4;

                    // set mode forced and read again
                    i2CBmp280.SetPowerMode(Bmx280PowerMode.Forced);

                    // wait for measurement to be performed
                    measurementTime = i2CBmp280.GetMeasurementDuration();
                    Thread.Sleep(measurementTime);

                    // read values
                    i2CBmp280.TryReadTemperature(out tempValue);
                    Console.WriteLine($"Temperature {tempValue.Celsius} \u00B0C");
                    i2CBmp280.TryReadPressure(out preValue);
                    Console.WriteLine($"Pressure {preValue.Hectopascal} hPa");
                    i2CBmp280.TryReadAltitude(defaultSeaLevelPressure, out altValue);
                    Console.WriteLine($"Altitude: {altValue} m");
                    Thread.Sleep(5000);
                }
            }
        }
    }
}