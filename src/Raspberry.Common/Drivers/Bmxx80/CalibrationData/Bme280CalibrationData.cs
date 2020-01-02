using Common.Drivers.Bmxx80.Register;
using System;

namespace Common.Drivers.Bmxx80.CalibrationData
{
	/// <summary>
	/// Calibration data for the BME280.
	/// </summary>
	internal class Bme280CalibrationData : Bmp280CalibrationData
	{
		public Byte DigH1 { get; set; }
		public Int16 DigH2 { get; set; }
		public UInt16 DigH3 { get; set; }
		public Int16 DigH4 { get; set; }
		public Int16 DigH5 { get; set; }
		public SByte DigH6 { get; set; }

		/// <summary>
		/// Read coefficient data from device.
		/// </summary>
		/// <param name="bmxx80Base">The <see cref="Bmxx80Base"/> to read coefficient data from.</param>
		protected internal override void ReadFromDevice(Bmxx80Base bmxx80Base)
		{
			// Read humidity calibration data.
			DigH1 = bmxx80Base.Read8BitsFromRegister((Byte)Bme280Register.DIG_H1);
			DigH2 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme280Register.DIG_H2);
			DigH3 = bmxx80Base.Read8BitsFromRegister((Byte)Bme280Register.DIG_H3);
			DigH4 = (Int16)((bmxx80Base.Read8BitsFromRegister((Byte)Bme280Register.DIG_H4) << 4) | (bmxx80Base.Read8BitsFromRegister((Byte)Bme280Register.DIG_H4 + 1) & 0xF));
			DigH5 = (Int16)((bmxx80Base.Read8BitsFromRegister((Byte)Bme280Register.DIG_H5 + 1) << 4) | (bmxx80Base.Read8BitsFromRegister((Byte)Bme280Register.DIG_H5) >> 4));
			DigH6 = (SByte)bmxx80Base.Read8BitsFromRegister((Byte)Bme280Register.DIG_H6);

			base.ReadFromDevice(bmxx80Base);
		}
	}
}