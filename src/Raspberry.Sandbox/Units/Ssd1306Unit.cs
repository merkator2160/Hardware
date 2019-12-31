using Common.Drivers.Ssd1306;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Ssd1306Unit
	{
		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var display = new Ssd1306DisplayDriver())
			{
				display.Clear();

				display.WriteImage(DisplayImages.Connected, 0, 0);
				display.WriteLine("Hello", 1);
				display.WriteLine("world", 2);
				display.WriteLine("!", 3);

				display.Refresh();

				//Thread.Sleep(2000);

				//display.TurnLightOff();

				//Thread.Sleep(2000);

				//display.TurnLightOn();
			}
		}
	}
}