using Common.Drivers.UpsHat;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class UpsHatUnit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var upsHat = new UpsHatDriver())
			{
				while(true)
				{
					var voltage = upsHat.ReadVoltage();
					var capacity = upsHat.ReadCapacity();

					Debug.WriteLine($"V: {voltage}, C: {capacity}");

					Thread.Sleep(1000);
				}
			}
		}
	}
}