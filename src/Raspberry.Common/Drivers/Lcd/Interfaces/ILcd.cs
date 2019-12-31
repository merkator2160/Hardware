using System;
using System.Drawing;

namespace Common.Drivers.Lcd.Interfaces
{
	public interface ILcd
	{
		Size Size { get; }
		Boolean BackLightOn { get; set; }
		Boolean DisplayOn { get; set; }
		Boolean UnderlineCursorVisible { get; set; }
		Boolean BlinkingCursorVisible { get; set; }
		Boolean AutoShift { get; set; }
		Boolean Increment { get; set; }

		void Clear();
		void Home();
		void SetCursorPosition(Int32 left, Int32 top);
		void ShiftDisplayLeft();
		void ShiftDisplayRight();
		void ShiftCursorLeft();
		void ShiftCursorRight();
		void CreateCustomCharacter(Byte location, params Byte[] characterMap);
		void CreateCustomCharacter(Byte location, ReadOnlySpan<Byte> characterMap);
		void Write(String value);
	}
}