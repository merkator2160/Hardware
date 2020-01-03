using System;

namespace Common.Drivers.Ssd1306.Old
{
	/// <summary>
	/// Column major, little endian.
	/// </summary>
	public class DisplayImage
	{
		public readonly UInt32 ImageWidthPx;
		public readonly UInt32 ImageHeightBytes;
		public readonly Byte[] ImageData;


		public DisplayImage(UInt32 imageHeightBytes, Byte[] imageData)
		{
			ImageWidthPx = (UInt32)imageData.Length / imageHeightBytes;
			ImageHeightBytes = imageHeightBytes;
			ImageData = imageData;
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
	}
}
