using System;
using System.Device.Gpio;

namespace Common.Models
{
	internal struct PinVector32
	{
		public PinVector32(UInt32 pins, UInt32 values)
		{
			Pins = pins;
			Values = values;
		}
		public PinVector32(ReadOnlySpan<PinValuePair> pinValues)
		{
			Pins = 0;
			Values = 0;

			foreach((var pin, var value) in pinValues)
			{
				if(pin < 0 || pin >= sizeof(UInt32) * 8)
				{
					throw new ArgumentOutOfRangeException(nameof(pinValues));
				}

				var bit = (UInt32)(1 << pin);
				Pins |= bit;
				if(value == PinValue.High)
				{
					Values |= bit;
				}
			}
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Bit vector of pin numbers from 0 (bit 0) to 31 (bit 31).
		/// </summary>
		public UInt32 Pins { get; set; }

		/// <summary>
		/// Bit vector of values for each pin number from 0 (bit 0) to 31 (bit 31).
		/// 1 is high, 0 is low.
		/// </summary>
		public UInt32 Values { get; set; }


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Convenience deconstructor. Allows using as a "return tuple".
		/// </summary>
		public void Deconstruct(out UInt32 pins, out UInt32 values)
		{
			pins = Pins;
			values = Values;
		}
	}
}