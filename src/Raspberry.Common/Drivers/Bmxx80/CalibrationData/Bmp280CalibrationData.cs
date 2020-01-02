using Common.Drivers.Bmxx80.Register;
using System;

namespace Common.Drivers.Bmxx80.CalibrationData
{
	/// <summary>
	/// Calibration data for the BMP280.
	/// </summary>
	internal class Bmp280CalibrationData : Bmxx80CalibrationData
	{
		/// <summary>
		/// Read coefficient data from device.
		/// </summary>
		/// <param name="bmxx80Base">The <see cref="Bmxx80Base"/> to read coefficient data from.</param>
		protected internal override void ReadFromDevice(Bmxx80Base bmxx80Base)
		{
			// Read temperature calibration data
			DigT1 = bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_T1);
			DigT2 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_T2);
			DigT3 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_T3);

			// Read pressure calibration data
			DigP1 = bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P1);
			DigP2 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P2);
			DigP3 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P3);
			DigP4 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P4);
			DigP5 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P5);
			DigP6 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P6);
			DigP7 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P7);
			DigP8 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P8);
			DigP9 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bmx280Register.DIG_P9);
		}
	}
}
