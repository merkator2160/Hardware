using Common.Drivers.Bmxx80.Register;
using System;

namespace Common.Drivers.Bmxx80.CalibrationData
{
	/// <summary>
	/// Calibration data for the <see cref="Bme680Driver"/>.
	/// </summary>
	internal class Bme680CalibrationData : Bmxx80CalibrationData
	{
		public Byte DigP10 { get; set; }

		public UInt16 DigH1 { get; set; }
		public UInt16 DigH2 { get; set; }
		public SByte DigH3 { get; set; }
		public SByte DigH4 { get; set; }
		public SByte DigH5 { get; set; }
		public Byte DigH6 { get; set; }
		public SByte DigH7 { get; set; }

		public SByte DigGh1 { get; set; }
		public Int16 DigGh2 { get; set; }
		public SByte DigGh3 { get; set; }

		public Byte ResHeatRange { get; set; }
		public SByte ResHeatVal { get; set; }
		public SByte RangeSwErr { get; set; }

		/// <summary>
		/// Read coefficient data from device.
		/// </summary>
		/// <param name="bmxx80Base">The <see cref="Bmxx80Base"/> to read coefficient data from.</param>
		protected internal override void ReadFromDevice(Bmxx80Base bmxx80Base)
		{
			// Read temperature calibration data.
			DigT1 = bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_T1);
			DigT2 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_T2);
			DigT3 = bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_T3);

			// Read humidity calibration data.
			DigH1 = (UInt16)((bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H1_MSB) << 4) | (bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H1_LSB) & (Byte)Bme680Mask.BIT_H1_DATA_MSK));
			DigH2 = (UInt16)((bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H2_MSB) << 4) | (bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H2_LSB) >> 4));
			DigH3 = (SByte)bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H3);
			DigH4 = (SByte)bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H4);
			DigH5 = (SByte)bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H5);
			DigH6 = bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H6);
			DigH7 = (SByte)(bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_H7));

			// Read pressure calibration data.
			DigP1 = bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_P1_LSB);
			DigP2 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_P2_LSB);
			DigP3 = bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_P3);
			DigP4 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_P4_LSB);
			DigP5 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_P5_LSB);
			DigP6 = bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_P6);
			DigP7 = bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_P7);
			DigP8 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_P8_LSB);
			DigP9 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_P9_LSB);
			DigP10 = bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_P10);

			// read gas calibration data.
			DigGh1 = (SByte)bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_GH1);
			DigGh2 = (Int16)bmxx80Base.Read16BitsFromRegister((Byte)Bme680Register.DIG_GH2);
			DigGh3 = (SByte)bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.DIG_GH3);

			// read heater calibration data
			ResHeatRange = (Byte)((bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.RES_HEAT_RANGE) & (Byte)Bme680Mask.RH_RANGE) >> 4);
			RangeSwErr = (SByte)((bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.RANGE_SW_ERR) & (Byte)Bme680Mask.RS_ERROR) >> 4);
			ResHeatVal = (SByte)bmxx80Base.Read8BitsFromRegister((Byte)Bme680Register.RES_HEAT_VAL);
		}
	}
}
