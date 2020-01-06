using Common.Const.Units;
using Common.Drivers.Bmxx80;
using Common.Drivers.Bmxx80.PowerMode;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Bmp280Unit
	{
		private readonly Pressure _defaultSeaLevelPressure = Pressure.MeanSeaLevel;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var driver = new Bme280Driver(0x76))
			{
				while(true)
				{
					ReadValues(driver);
					PrintValues(driver);

					Thread.Sleep(1000);
				}
			}
		}
		private static void ReadValues(Bme280Driver driver)
		{
			driver.SetPowerMode(Bmx280PowerMode.Forced);
			Thread.Sleep(driver.GetMeasurementDuration());
		}
		private void PrintValues(Bme280Driver driver)
		{
			driver.TryReadTemperature(out var tempValue);
			driver.TryReadPressure(out var preValue);
			driver.TryReadAltitude(_defaultSeaLevelPressure, out var altValue);
			driver.TryReadHumidity(out var humValue);

			Debug.WriteLine($"Temp: {tempValue.Celsius:F} \u00B0C, Pr: {preValue.Hectopascal:F} hPa, Alt: {altValue:F} meters, Hum: {humValue:F} %");
		}
	}
}