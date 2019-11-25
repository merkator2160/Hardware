using Common.Drivers.Ssd1306;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Ssd1306Unit
	{
		private static Display display;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			display = new Display();
			display.Init(true);

			display.ClearDisplayBuf();

			// Row 0, and image
			display.WriteImageDisplayBuf(DisplayImages.Connected, 0, 0);

			// Row 1 - 3
			display.WriteLineDisplayBuf("Hello", 0, 1);
			display.WriteLineDisplayBuf("World", 0, 2);

			display.DisplayUpdate();
		}
	}
}