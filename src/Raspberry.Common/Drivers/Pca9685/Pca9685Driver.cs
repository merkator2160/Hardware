using Common.Drivers.Pca9685.Interfaces;
using Common.Drivers.Pca9685.Registers;
using Common.Helpers;
using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Device.Pwm;
using System.Diagnostics;

namespace Common.Drivers.Pca9685
{
	/// <summary>
	/// PCA9685 PWM LED/servo controller
	/// </summary>
	public class Pca9685Driver : IPca9685, IDisposable
	{
		public const Byte _i2cAddressBase = 64;

		private I2cDevice _device;
		private readonly Boolean _usingExternalClock;
		private UInt16 _createdChannelsMask;


		public Pca9685Driver() : this(_i2cAddressBase)
		{

		}
		public Pca9685Driver(Int32 address, Int32 busId = 1, Double pwmFrequency = -1, Double dutyCycleAllChannels = -1, Boolean usingExternalClock = false)
		{
			_device = I2cDevice.Create(new I2cConnectionSettings(busId, address));
			_usingExternalClock = usingExternalClock;

			var mode1 = Mode1.SLEEP | Mode1.ALLCALL | Mode1.AI;

			if(usingExternalClock)
			{
				mode1 |= Mode1.EXTCLK;
			}

			var mode2 = Mode2.OUTDRV;

			WriteByte(Register.MODE1, (Byte)mode1);
			WriteByte(Register.MODE2, (Byte)mode2);

			if(pwmFrequency == -1)
			{
				_prescale = ReadByte(Register.PRESCALE);
			}
			else
			{
				PwmFrequency = pwmFrequency;
			}

			if(dutyCycleAllChannels != -1)
			{
				SetDutyCycleAllChannels(dutyCycleAllChannels);
			}

			mode1 &= ~Mode1.SLEEP;
			WriteByte(Register.MODE1, (Byte)mode1);
			DelayHelper.DelayMicroseconds(500, allowThreadYield: true);
		}


		// IPca9685 ///////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Get clock frequency (Hz). Only set if you are using external clock.
		/// </summary>
		public Double ClockFrequency
		{
			get => _clockFrequency;
			set
			{
				if(!_usingExternalClock)
				{
					throw new InvalidOperationException("Clock frequency can only be set when using external oscillator.");
				}

				_clockFrequency = value;
			}
		}
		private Double _clockFrequency = 25_000_000;

		/// <summary>
		/// Set PWM frequency or get effective value.
		/// </summary>
		/// <remarks>
		/// Value of the effective frequency may be different than desired frequency.
		/// Read the property in order to get the actual value
		/// </remarks>
		public Double PwmFrequency
		{
			get => PrescaleToFrequency(_prescale);
			set => Prescale = FrequencyToPrescale(value);
		}


		/// <summary>
		/// Sets duty cycle on specified channel.
		/// </summary>
		/// <param name="channel">Selected channel</param>
		/// <param name="dutyCycle">Value to set duty cycle to</param>
		/// <remarks>Throws <see cref="InvalidOperationException"/> if specified channel is created with <see cref="CreatePwmChannel"/></remarks>
		public void SetDutyCycle(Int32 channel, Double dutyCycle)
		{
			CheckChannel(channel);

			if(IsChannelCreated(channel))
			{
				throw new InvalidOperationException("Cannot set duty cycle directly when PwmChannel is created. Use PwmChannel instance instead.");
			}

			SetDutyCycleInternal(channel, dutyCycle);
		}

		/// <summary>
		/// Gets duty cycle on specified channel
		/// </summary>
		/// <param name="channel">selected channel</param>
		/// <returns>Value of duty cycle in 0.0 - 1.0 range</returns>
		public Double GetDutyCycle(Int32 channel)
		{
			CheckChannel(channel);

			var offset = 4 * channel;

			var on = ReadUInt16((Register)((Byte)Register.LED0_ON_L + offset));
			var off = ReadUInt16((Register)((Byte)Register.LED0_OFF_L + offset));
			return OnOffToDutyCycle(on, off);
		}

		/// <summary>
		/// Sets duty cycles on all channels
		/// </summary>
		/// <param name="dutyCycle">Duty cycle value (0.0 - 1.0)</param>
		/// <remarks>Throws <see cref="InvalidOperationException"/> if any of the channels is created with <see cref="CreatePwmChannel"/></remarks>
		public void SetDutyCycleAllChannels(Double dutyCycle)
		{
			if(IsAnyChannelCreated())
			{
				throw new InvalidOperationException("Cannot set duty cycle directly when any of the channels has corresponding PwmChannel instance created.");
			}

			CheckDutyCycle(dutyCycle);
			(var on, var off) = DutyCycleToOnOff(dutyCycle);
			SetOnOffTimeAllChannels(on, off);
		}

		/// <summary>
		/// Creates PwmChannel instance from selected channel
		/// </summary>
		/// <param name="channel">Channel number (0-15)</param>
		/// <returns>PwmChannel instance</returns>
		/// <remarks>Channel is already started when constructed.</remarks>
		public PwmChannel CreatePwmChannel(Int32 channel)
		{
			CheckChannel(channel);

			if(IsChannelCreated(channel))
			{
				throw new ArgumentException("Only one instance of the channel can be created at the same time.", nameof(channel));
			}

			SetChannelAsCreated(channel);

			return new Pca9685PwmChannel(this, channel);
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Set PWM frequency using prescale value or get the value.
		/// </summary>
		protected Byte Prescale
		{
			get => _prescale;
			set
			{
				var v = value < 3 ? (Byte)3 : value;  // min value is 3
				SetPrescale(v);
				_prescale = v;
			}
		}
		private Byte _prescale;


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		internal void SetDutyCycleInternal(Int32 channel, Double dutyCycle)
		{
			CheckDutyCycle(dutyCycle);

			(var on, var off) = DutyCycleToOnOff(dutyCycle);
			SetOnOffTime(channel, on, off);
		}
		private void SetOnOffTime(Int32 channel, UInt16 on, UInt16 off)
		{
			// on and off are 13-bit values (12 bit value + 1bit full on/off override)
			Debug.Assert((on & 0b1111111111111) == on, "On has invalid value");
			Debug.Assert((off & 0b1111111111111) == off, "Off has invalid value");
			Debug.Assert((channel & 0xF) == channel, "Unknown channel");

			var offset = 4 * channel;

			WriteUInt16((Register)((Byte)Register.LED0_ON_L + offset), on);
			WriteUInt16((Register)((Byte)Register.LED0_OFF_L + offset), off);
		}
		private void SetOnOffTimeAllChannels(UInt16 on, UInt16 off)
		{
			// on and off are 13-bit values (12 bit value + 1bit full on/off override)
			Debug.Assert((on & 0b1111111111111) == on, "On has invalid value");
			Debug.Assert((off & 0b1111111111111) == off, "Off has invalid value");

			WriteUInt16(Register.ALL_LED_ON_L, on);
			WriteUInt16(Register.ALL_LED_OFF_L, off);
		}
		private Byte FrequencyToPrescale(Double freqHz)
		{
			var desiredPrescale = Math.Round(ClockFrequency / 4096 / freqHz - 1);
			return (Byte)Math.Clamp(desiredPrescale, Byte.MinValue, Byte.MaxValue);
		}
		private Double PrescaleToFrequency(Byte prescale)
		{
			return ClockFrequency / 4096 / (prescale + 1.0);
		}
		private void SetPrescale(Byte prescale)
		{
			var oldmode = (Mode1)ReadByte(Register.MODE1);

			if(oldmode.HasFlag(Mode1.SLEEP))
			{
				WriteByte(Register.PRESCALE, prescale);
			}
			else
			{
				WriteByte(Register.MODE1, (Byte)(oldmode | Mode1.SLEEP));
				WriteByte(Register.PRESCALE, prescale);
				WriteByte(Register.MODE1, (Byte)oldmode);
				DelayHelper.DelayMicroseconds(500, true);
			}
		}
		private Boolean IsChannelCreated(Int32 channel) => (_createdChannelsMask & (1 << channel)) != 0;
		private Boolean IsAnyChannelCreated() => _createdChannelsMask != 0;
		private void SetChannelAsCreated(Int32 channel) => _createdChannelsMask |= (UInt16)(1 << channel);
		internal void SetChannelAsDestroyed(Int32 channel) => _createdChannelsMask &= (UInt16)(~(1 << channel));
		private Byte ReadByte(Register register)
		{
			_device.WriteByte((Byte)register);

			return _device.ReadByte();
		}
		private UInt16 ReadUInt16(Register register)
		{
			_device.WriteByte((Byte)register);

			var bytes = new Byte[2];
			_device.Read(bytes);

			return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
		}
		private void WriteByte(Register register, Byte data)
		{
			Span<Byte> bytes = stackalloc Byte[2];
			bytes[0] = (Byte)register;
			bytes[1] = data;
			_device.Write(bytes);
		}
		private void WriteUInt16(Register register, UInt16 value)
		{
			WriteByte(register, (Byte)value);
			WriteByte(register + 1, (Byte)(value >> 8));

			Span<Byte> bytes = stackalloc Byte[3];
			bytes[0] = (Byte)register;
			BinaryPrimitives.WriteUInt16LittleEndian(bytes.Slice(1), value);
			_device.Write(bytes);
		}
		private static (UInt16 on, UInt16 off) DutyCycleToOnOff(Double dutyCycle)
		{
			Debug.Assert(dutyCycle >= 0.0 && dutyCycle <= 1.0, "Duty cycle must be between 0 and 1");

			// there are actually 4097 values in the set but we can do edge values
			// using 13th bit which overrides to always on/off
			var dutyCycleSampled = (UInt16)Math.Round(dutyCycle * 4096);

			if(dutyCycleSampled == 0)
			{
				return (0, 1 << 12);
			}
			else if(dutyCycleSampled == 4096)
			{
				return (1 << 12, 0);
			}
			else
			{
				return (0, dutyCycleSampled);
			}
		}
		private static Double OnOffToDutyCycle(UInt16 on, UInt16 off)
		{
			UInt16 OnOffToDutyCycleSampled(UInt16 onCycles, UInt16 offCycles)
			{
				const UInt16 Max = (UInt16)(1 << 12);
				if(onCycles == 0)
				{
					return (offCycles == Max) ? (UInt16)0 : offCycles;
				}
				else if(onCycles == Max && offCycles == 0)
				{
					return 4096;
				}

				// we didn't set this value anywhere in the code
				throw new InvalidOperationException($"Unexpected value of duty cycle ({onCycles}, {offCycles})");
			}

			return OnOffToDutyCycleSampled(on, off) / 4096.0;
		}
		private static void CheckDutyCycle(Double dutyCycle)
		{
			if(dutyCycle < 0.0 || dutyCycle > 1.0)
			{
				throw new ArgumentOutOfRangeException(nameof(dutyCycle), dutyCycle, "Value must be between 0.0 and 1.0.");
			}
		}
		private static void CheckChannel(Int32 channel)
		{
			if(channel < 0 || channel >= 16)
			{
				throw new ArgumentOutOfRangeException(nameof(channel), channel, "Channel must be a value from 0 to 15.");
			}
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		public void Dispose()
		{
			_device?.Dispose();
			_device = null;
		}
	}
}
