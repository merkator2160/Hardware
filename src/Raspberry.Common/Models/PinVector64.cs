using System;
using System.Device.Gpio;

namespace Common.Models
{
	internal struct PinVector64
	{
		public PinVector64(UInt64 pins, UInt64 values)
		{
			Pins = pins;
			Values = values;
		}
		public PinVector64(ReadOnlySpan<PinValuePair> pinValues)
		{
			Pins = 0;
			Values = 0;

			foreach((var pin, var value) in pinValues)
			{
				if(pin < 0 || pin >= sizeof(UInt64) * 8)
				{
					throw new ArgumentOutOfRangeException(nameof(pinValues));
				}

				var bit = (UInt64)(1 << pin);
				Pins |= bit;
				if(value == PinValue.High)
				{
					Values |= bit;
				}
			}
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Bit vector of pin numbers from 0 (bit 0) to 63 (bit 63).
		/// </summary>
		public UInt64 Pins { get; set; }

		/// <summary>
		/// Bit vector of values for each pin number from 0 (bit 0) to 63 (bit 63).
		/// 1 is high, 0 is low.
		/// </summary>
		public UInt64 Values { get; set; }


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convenience deconstructor. Allows using as a "return tuple".
		/// </summary>
		public void Deconstruct(out UInt64 pins, out UInt64 values)
		{
			pins = Pins;
			values = Values;
		}
	}
}