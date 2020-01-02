using Common.Drivers.Rtc.Ds1307;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Ds1307Unit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var rtc = new Ds1307Driver(104))
			{
				rtc.DateTime = DateTime.Now;

				while(true)
				{
					var dt = rtc.DateTime;

					Debug.WriteLine($"Time: {dt:yyyy/MM/dd HH:mm:ss}");

					Thread.Sleep(1000);
				}
			}
		}
	}
}