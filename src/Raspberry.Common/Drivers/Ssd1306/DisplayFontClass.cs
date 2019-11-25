using System;

namespace Common.Drivers.Ssd1306
{
	/// <summary>
	/// FontCharacter descriptor contains font information for a  single character
	/// </summary>
	public class FontCharacterDescriptor
	{
		public readonly Char Character;
		public readonly UInt32 CharacterWidthPx;
		public readonly UInt32 CharacterHeightBytes;
		public readonly Byte[] CharacterData;

		public FontCharacterDescriptor(Char chr, UInt32 charHeightBytes, Byte[] charData)
		{
			Character = chr;
			CharacterWidthPx = (UInt32)charData.Length / charHeightBytes;
			CharacterHeightBytes = charHeightBytes;
			CharacterData = charData;
		}
	}
}
