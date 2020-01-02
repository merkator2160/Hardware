using Common.Drivers.Rtc.Ds3231;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Ds3231Unit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var rtc = new Ds3231Driver(104))
			{
				Debug.WriteLine(DateTime.Now);

				rtc.DateTime = DateTime.Now;

				while(true)
				{
					var dt = rtc.DateTime;
					Debug.WriteLine($"Time: {dt:yyyy/MM/dd HH:mm:ss}, Temperature: {rtc.Temperature.Celsius} ℃");
					Thread.Sleep(1000);
				}
			}
		}
	}
}