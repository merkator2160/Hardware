using Common.Drivers.Lcd;
using System.Diagnostics;
using System.Drawing;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class Lcd2004Unit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var lcd = new Hd44780Driver(new Size(20, 4), LcdInterfaceBase.CreateI2c(39, 1, false)))
			{
				lcd.Clear();

				lcd.Write("Hello World!");
				lcd.SetCursorPosition(0, 1);
				lcd.Write("Hello World!");
				lcd.SetCursorPosition(0, 2);
				lcd.Write("Hello World!");
				lcd.SetCursorPosition(0, 3);
				lcd.Write("Hello World!");

				Debug.WriteLine("Hello World!");
			}
		}
	}
}