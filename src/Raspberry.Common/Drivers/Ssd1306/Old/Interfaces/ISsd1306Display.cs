using System;

namespace Common.Drivers.Ssd1306.Old.Interfaces
{
	public interface ISsd1306Display
	{
		void Refresh();
		void WriteLine(String text, UInt32 row);
		void WriteLine(String text, UInt32 row, UInt32 col);
		UInt32 WriteChar(Char character, UInt32 row, UInt32 col);
		UInt32 WriteImage(DisplayImage img, UInt32 row, UInt32 col);
		void Clear();
		void TurnLightOn();
		void TurnLightOff();
	}
}