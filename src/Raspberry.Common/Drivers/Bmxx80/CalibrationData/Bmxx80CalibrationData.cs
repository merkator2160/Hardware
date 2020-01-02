using System;

namespace Common.Drivers.Bmxx80.CalibrationData
{
	/// <summary>
	/// Calibration data for the Bmxx80 family.
	/// </summary>
	internal abstract class Bmxx80CalibrationData
	{
		public UInt16 DigT1 { get; set; }
		public Int16 DigT2 { get; set; }
		public Int16 DigT3 { get; set; }

		public UInt16 DigP1 { get; set; }
		public Int16 DigP2 { get; set; }
		public Int16 DigP3 { get; set; }
		public Int16 DigP4 { get; set; }
		public Int16 DigP5 { get; set; }
		public Int16 DigP6 { get; set; }
		public Int16 DigP7 { get; set; }
		public Int16 DigP8 { get; set; }
		public Int16 DigP9 { get; set; }

		/// <summary>
		/// Read coefficient data from device.
		/// </summary>
		/// <param name="bmxx80Base">The <see cref="Bmxx80Base"/> to read coefficient data from.</param>
		protected internal abstract void ReadFromDevice(Bmxx80Base bmxx80Base);
	}
}
