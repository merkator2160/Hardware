﻿using System;
using System.Linq;

namespace Common.Drivers.Ssd1306.Old
{
	/// <summary>
	/// This class contains the character data needed to output render text on the display
	/// </summary>
	static class DisplayFontTable
	{
		public static readonly UInt32 FontHeightBytes = 2;  // Height of the characters. A value of 2 would indicate a 16 (2*8) pixel tall character
		public static readonly UInt32 FontCharSpacing = 1;  // Number of blank horizontal pixels to insert between adjacent characters

		/// <summary>
		/// Takes and returns the character descriptor for the corresponding Char if it exists
		/// </summary>
		public static FontCharacterDescriptor GetCharacterDescriptor(Char chr)
		{
			return FontTable.FirstOrDefault(CharDescriptor => CharDescriptor.Character == chr);
		}

		/// <summary>
		/// Table with all the character data
		/// </summary>
		private static readonly FontCharacterDescriptor[] FontTable =
		{
			new FontCharacterDescriptor(' ' ,FontHeightBytes,new Byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('!' ,FontHeightBytes,new Byte[]{0xFE,0x05}),
			new FontCharacterDescriptor('"' ,FontHeightBytes,new Byte[]{0x1E,0x00,0x1E,0x00,0x00,0x00}),
			new FontCharacterDescriptor('#' ,FontHeightBytes,new Byte[]{0x80,0x90,0xF0,0x9E,0xF0,0x9E,0x10,0x00,0x07,0x00,0x07,0x00,0x00,0x00}),
			new FontCharacterDescriptor('$' ,FontHeightBytes,new Byte[]{0x38,0x44,0xFE,0x44,0x98,0x02,0x04,0x0F,0x04,0x03}),
			new FontCharacterDescriptor('%' ,FontHeightBytes,new Byte[]{0x0C,0x12,0x12,0x8C,0x40,0x20,0x10,0x88,0x84,0x00,0x00,0x02,0x01,0x00,0x00,0x00,0x03,0x04,0x04,0x03}),
			new FontCharacterDescriptor('&' ,FontHeightBytes,new Byte[]{0x80,0x5C,0x22,0x62,0x9C,0x00,0x00,0x03,0x04,0x04,0x04,0x05,0x02,0x05}),
			new FontCharacterDescriptor('\'',FontHeightBytes,new Byte[]{0x1E,0x00}),
			new FontCharacterDescriptor('(' ,FontHeightBytes,new Byte[]{0xF0,0x0C,0x02,0x07,0x18,0x20}),
			new FontCharacterDescriptor(')' ,FontHeightBytes,new Byte[]{0x02,0x0C,0xF0,0x20,0x18,0x07}),
			new FontCharacterDescriptor('*' ,FontHeightBytes,new Byte[]{0x14,0x18,0x0E,0x18,0x14,0x00,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('+' ,FontHeightBytes,new Byte[]{0x40,0x40,0xF0,0x40,0x40,0x00,0x00,0x01,0x00,0x00}),
			new FontCharacterDescriptor(',' ,FontHeightBytes,new Byte[]{0x00,0x00,0x08,0x04}),
			new FontCharacterDescriptor('-' ,FontHeightBytes,new Byte[]{0x40,0x40,0x40,0x40,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('.' ,FontHeightBytes,new Byte[]{0x00,0x04}),
			new FontCharacterDescriptor('/' ,FontHeightBytes,new Byte[]{0x00,0x80,0x70,0x0E,0x1C,0x03,0x00,0x00}),
			new FontCharacterDescriptor('0' ,FontHeightBytes,new Byte[]{0xFC,0x02,0x02,0x02,0xFC,0x03,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('1' ,FontHeightBytes,new Byte[]{0x04,0x04,0xFE,0x00,0x00,0x07}),
			new FontCharacterDescriptor('2' ,FontHeightBytes,new Byte[]{0x0C,0x82,0x42,0x22,0x1C,0x07,0x04,0x04,0x04,0x04}),
			new FontCharacterDescriptor('3' ,FontHeightBytes,new Byte[]{0x04,0x02,0x22,0x22,0xDC,0x02,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('4' ,FontHeightBytes,new Byte[]{0xC0,0xA0,0x98,0x84,0xFE,0x00,0x00,0x00,0x00,0x07}),
			new FontCharacterDescriptor('5' ,FontHeightBytes,new Byte[]{0x7E,0x22,0x22,0x22,0xC2,0x02,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('6' ,FontHeightBytes,new Byte[]{0xFC,0x42,0x22,0x22,0xC4,0x03,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('7' ,FontHeightBytes,new Byte[]{0x02,0x02,0xC2,0x32,0x0E,0x00,0x07,0x00,0x00,0x00}),
			new FontCharacterDescriptor('8' ,FontHeightBytes,new Byte[]{0xDC,0x22,0x22,0x22,0xDC,0x03,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('9' ,FontHeightBytes,new Byte[]{0x3C,0x42,0x42,0x22,0xFC,0x02,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor(':' ,FontHeightBytes,new Byte[]{0x10,0x04}),
			new FontCharacterDescriptor(';' ,FontHeightBytes,new Byte[]{0x00,0x10,0x08,0x04}),
			new FontCharacterDescriptor('<' ,FontHeightBytes,new Byte[]{0x40,0xE0,0xB0,0x18,0x08,0x00,0x00,0x01,0x03,0x02}),
			new FontCharacterDescriptor('=' ,FontHeightBytes,new Byte[]{0xA0,0xA0,0xA0,0xA0,0xA0,0x00,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('>' ,FontHeightBytes,new Byte[]{0x08,0x18,0xB0,0xE0,0x40,0x02,0x03,0x01,0x00,0x00}),
			new FontCharacterDescriptor('?' ,FontHeightBytes,new Byte[]{0x0C,0x02,0xC2,0x22,0x1C,0x00,0x00,0x05,0x00,0x00}),
			new FontCharacterDescriptor('@' ,FontHeightBytes,new Byte[]{0xF0,0x0C,0x02,0x02,0xE1,0x11,0x11,0x91,0x72,0x02,0x0C,0xF0,0x00,0x03,0x04,0x04,0x08,0x09,0x09,0x08,0x09,0x05,0x05,0x00}),
			new FontCharacterDescriptor('A' ,FontHeightBytes,new Byte[]{0x00,0x80,0xE0,0x98,0x86,0x98,0xE0,0x80,0x00,0x06,0x01,0x00,0x00,0x00,0x00,0x00,0x01,0x06}),
			new FontCharacterDescriptor('B' ,FontHeightBytes,new Byte[]{0xFE,0x22,0x22,0x22,0x22,0x22,0xDC,0x07,0x04,0x04,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('C' ,FontHeightBytes,new Byte[]{0xF8,0x04,0x02,0x02,0x02,0x02,0x04,0x08,0x01,0x02,0x04,0x04,0x04,0x04,0x02,0x01}),
			new FontCharacterDescriptor('D' ,FontHeightBytes,new Byte[]{0xFE,0x02,0x02,0x02,0x02,0x02,0x04,0xF8,0x07,0x04,0x04,0x04,0x04,0x04,0x02,0x01}),
			new FontCharacterDescriptor('E' ,FontHeightBytes,new Byte[]{0xFE,0x22,0x22,0x22,0x22,0x22,0x02,0x07,0x04,0x04,0x04,0x04,0x04,0x04}),
			new FontCharacterDescriptor('F' ,FontHeightBytes,new Byte[]{0xFE,0x22,0x22,0x22,0x22,0x22,0x02,0x07,0x00,0x00,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('G' ,FontHeightBytes,new Byte[]{0xF8,0x04,0x02,0x02,0x02,0x42,0x44,0xC8,0x01,0x02,0x04,0x04,0x04,0x04,0x02,0x07}),
			new FontCharacterDescriptor('H' ,FontHeightBytes,new Byte[]{0xFE,0x20,0x20,0x20,0x20,0x20,0x20,0xFE,0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x07}),
			new FontCharacterDescriptor('I' ,FontHeightBytes,new Byte[]{0xFE,0x07}),
			new FontCharacterDescriptor('J' ,FontHeightBytes,new Byte[]{0x00,0x00,0x00,0x00,0xFE,0x03,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('K' ,FontHeightBytes,new Byte[]{0xFE,0x20,0x50,0x88,0x04,0x02,0x00,0x07,0x00,0x00,0x00,0x01,0x02,0x04}),
			new FontCharacterDescriptor('L' ,FontHeightBytes,new Byte[]{0xFE,0x00,0x00,0x00,0x00,0x00,0x07,0x04,0x04,0x04,0x04,0x04}),
			new FontCharacterDescriptor('M' ,FontHeightBytes,new Byte[]{0xFE,0x18,0x60,0x80,0x00,0x80,0x60,0x18,0xFE,0x07,0x00,0x00,0x01,0x06,0x01,0x00,0x00,0x07}),
			new FontCharacterDescriptor('N' ,FontHeightBytes,new Byte[]{0xFE,0x04,0x18,0x20,0x40,0x80,0x00,0xFE,0x07,0x00,0x00,0x00,0x00,0x01,0x02,0x07}),
			new FontCharacterDescriptor('O' ,FontHeightBytes,new Byte[]{0xF8,0x04,0x02,0x02,0x02,0x02,0x04,0xF8,0x01,0x02,0x04,0x04,0x04,0x04,0x02,0x01}),
			new FontCharacterDescriptor('P' ,FontHeightBytes,new Byte[]{0xFE,0x42,0x42,0x42,0x42,0x42,0x24,0x18,0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('Q' ,FontHeightBytes,new Byte[]{0xF8,0x04,0x02,0x02,0x02,0x02,0x04,0xF8,0x01,0x02,0x04,0x04,0x04,0x05,0x02,0x05}),
			new FontCharacterDescriptor('R' ,FontHeightBytes,new Byte[]{0xFE,0x42,0x42,0x42,0x42,0x42,0x64,0x98,0x00,0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x03,0x04}),
			new FontCharacterDescriptor('S' ,FontHeightBytes,new Byte[]{0x1C,0x22,0x22,0x22,0x42,0x42,0x8C,0x03,0x04,0x04,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('T' ,FontHeightBytes,new Byte[]{0x02,0x02,0x02,0x02,0xFE,0x02,0x02,0x02,0x02,0x00,0x00,0x00,0x00,0x07,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('U' ,FontHeightBytes,new Byte[]{0xFE,0x00,0x00,0x00,0x00,0x00,0x00,0xFE,0x01,0x02,0x04,0x04,0x04,0x04,0x02,0x01}),
			new FontCharacterDescriptor('V' ,FontHeightBytes,new Byte[]{0x06,0x18,0x60,0x80,0x00,0x80,0x60,0x18,0x06,0x00,0x00,0x00,0x01,0x06,0x01,0x00,0x00,0x00}),
			new FontCharacterDescriptor('W' ,FontHeightBytes,new Byte[]{0x0E,0x30,0xC0,0x00,0xC0,0x30,0x0E,0x30,0xC0,0x00,0xC0,0x30,0x0E,0x00,0x00,0x01,0x06,0x01,0x00,0x00,0x00,0x01,0x06,0x01,0x00,0x00}),
			new FontCharacterDescriptor('X' ,FontHeightBytes,new Byte[]{0x06,0x08,0x90,0x60,0x60,0x90,0x08,0x06,0x06,0x01,0x00,0x00,0x00,0x00,0x01,0x06}),
			new FontCharacterDescriptor('Y' ,FontHeightBytes,new Byte[]{0x06,0x08,0x10,0x20,0xC0,0x20,0x10,0x08,0x06,0x00,0x00,0x00,0x00,0x07,0x00,0x00,0x00,0x00}),
			new FontCharacterDescriptor('Z' ,FontHeightBytes,new Byte[]{0x02,0x82,0x42,0x22,0x1A,0x06,0x06,0x05,0x04,0x04,0x04,0x04}),
			new FontCharacterDescriptor('[' ,FontHeightBytes,new Byte[]{0xFE,0x02,0x02,0x3F,0x20,0x20}),
			new FontCharacterDescriptor('\\',FontHeightBytes,new Byte[]{0x0E,0x70,0x80,0x00,0x00,0x00,0x03,0x1C}),
			new FontCharacterDescriptor('^' ,FontHeightBytes,new Byte[]{0x02,0x02,0xFE,0x20,0x20,0x3F}),
			new FontCharacterDescriptor('_' ,FontHeightBytes,new Byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x10,0x10,0x10,0x10,0x10,0x10,0x10}),
			new FontCharacterDescriptor('`' ,FontHeightBytes,new Byte[]{0x02,0x04,0x00,0x00}),
			new FontCharacterDescriptor('a' ,FontHeightBytes,new Byte[]{0xA0,0x50,0x50,0x50,0x50,0xE0,0x00,0x03,0x04,0x04,0x04,0x04,0x03,0x04}),
			new FontCharacterDescriptor('b' ,FontHeightBytes,new Byte[]{0xFE,0x20,0x10,0x10,0x10,0xE0,0x07,0x02,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('c' ,FontHeightBytes,new Byte[]{0xE0,0x10,0x10,0x10,0x10,0x20,0x03,0x04,0x04,0x04,0x04,0x02}),
			new FontCharacterDescriptor('d' ,FontHeightBytes,new Byte[]{0xE0,0x10,0x10,0x10,0x20,0xFE,0x03,0x04,0x04,0x04,0x02,0x07}),
			new FontCharacterDescriptor('e' ,FontHeightBytes,new Byte[]{0xE0,0x90,0x90,0x90,0x90,0xE0,0x03,0x04,0x04,0x04,0x04,0x02}),
			new FontCharacterDescriptor('f' ,FontHeightBytes,new Byte[]{0x10,0xFC,0x12,0x00,0x07,0x00}),
			new FontCharacterDescriptor('g' ,FontHeightBytes,new Byte[]{0xE0,0x10,0x10,0x10,0x20,0xF0,0x03,0x24,0x24,0x24,0x22,0x1F}),
			new FontCharacterDescriptor('h' ,FontHeightBytes,new Byte[]{0xFE,0x20,0x10,0x10,0xE0,0x07,0x00,0x00,0x00,0x07}),
			new FontCharacterDescriptor('i' ,FontHeightBytes,new Byte[]{0xF2,0x07}),
			new FontCharacterDescriptor('j' ,FontHeightBytes,new Byte[]{0x00,0xF2,0x20,0x1F}),
			new FontCharacterDescriptor('k' ,FontHeightBytes,new Byte[]{0xFE,0x80,0xC0,0x20,0x10,0x00,0x07,0x00,0x00,0x01,0x02,0x04}),
			new FontCharacterDescriptor('l' ,FontHeightBytes,new Byte[]{0xFE,0x07}),
			new FontCharacterDescriptor('m' ,FontHeightBytes,new Byte[]{0xF0,0x20,0x10,0x10,0xE0,0x20,0x10,0x10,0xE0,0x07,0x00,0x00,0x00,0x07,0x00,0x00,0x00,0x07}),
			new FontCharacterDescriptor('n' ,FontHeightBytes,new Byte[]{0xF0,0x20,0x10,0x10,0xE0,0x07,0x00,0x00,0x00,0x07}),
			new FontCharacterDescriptor('o' ,FontHeightBytes,new Byte[]{0xE0,0x10,0x10,0x10,0x10,0xE0,0x03,0x04,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('p' ,FontHeightBytes,new Byte[]{0xF0,0x20,0x10,0x10,0x10,0xE0,0x3F,0x02,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('q' ,FontHeightBytes,new Byte[]{0xE0,0x10,0x10,0x10,0x20,0xF0,0x03,0x04,0x04,0x04,0x02,0x3F}),
			new FontCharacterDescriptor('r' ,FontHeightBytes,new Byte[]{0xF0,0x20,0x10,0x07,0x00,0x00}),
			new FontCharacterDescriptor('s' ,FontHeightBytes,new Byte[]{0x60,0x90,0x90,0x90,0x20,0x02,0x04,0x04,0x04,0x03}),
			new FontCharacterDescriptor('t' ,FontHeightBytes,new Byte[]{0x10,0xFC,0x10,0x00,0x03,0x04}),
			new FontCharacterDescriptor('u' ,FontHeightBytes,new Byte[]{0xF0,0x00,0x00,0x00,0xF0,0x03,0x04,0x04,0x02,0x07}),
			new FontCharacterDescriptor('v' ,FontHeightBytes,new Byte[]{0x30,0xC0,0x00,0x00,0x00,0xC0,0x30,0x00,0x00,0x03,0x04,0x03,0x00,0x00}),
			new FontCharacterDescriptor('w' ,FontHeightBytes,new Byte[]{0x30,0xC0,0x00,0xC0,0x30,0xC0,0x00,0xC0,0x30,0x00,0x01,0x06,0x01,0x00,0x01,0x06,0x01,0x00}),
			new FontCharacterDescriptor('x' ,FontHeightBytes,new Byte[]{0x10,0x20,0xC0,0xC0,0x20,0x10,0x04,0x02,0x01,0x01,0x02,0x04}),
			new FontCharacterDescriptor('y' ,FontHeightBytes,new Byte[]{0x30,0xC0,0x00,0x00,0x00,0xC0,0x30,0x20,0x20,0x13,0x0C,0x03,0x00,0x00}),
			new FontCharacterDescriptor('z' ,FontHeightBytes,new Byte[]{0x10,0x90,0x50,0x30,0x06,0x05,0x04,0x04}),
			new FontCharacterDescriptor('{' ,FontHeightBytes,new Byte[]{0x80,0x80,0x7C,0x02,0x02,0x00,0x00,0x1F,0x20,0x20}),
			new FontCharacterDescriptor('|' ,FontHeightBytes,new Byte[]{0xFE,0x3F}),
			new FontCharacterDescriptor('}' ,FontHeightBytes,new Byte[]{0x02,0x02,0x7C,0x80,0x80,0x20,0x20,0x1F,0x00,0x00}),
			new FontCharacterDescriptor('~' ,FontHeightBytes,new Byte[]{0x0C,0x02,0x02,0x04,0x08,0x08,0x06,0x00,0x00,0x00,0x00,0x00,0x00,0x00}),
		};
	}
}