using Common.Const.Units;
using Common.Drivers.Bmxx80.FilteringMode;
using Common.Drivers.Bmxx80.PowerMode;
using Common.Drivers.Bmxx80.Register;
using System;
using System.Device.I2c;
using System.IO;

namespace Common.Drivers.Bmxx80
{
	/// <summary>
	/// Represents the core functionality of the Bmx280 family.
	/// </summary>
	public abstract class Bmx280Base : Bmxx80Base
	{
		/// <summary>
		/// Default I2C bus address.
		/// </summary>
		public const Byte DefaultI2cAddress = 0x77;

		/// <summary>
		/// Secondary I2C bus address.
		/// </summary>
		public const Byte SecondaryI2cAddress = 0x76;

		/// <summary>
		/// Converts oversampling to needed measurement cycles for that oversampling.
		/// </summary>
		protected static readonly Int32[] s_osToMeasCycles = { 0, 7, 9, 14, 23, 44 };

		private Bmx280FilteringMode _filteringMode;
		private StandbyTime _standbyTime;

		/// <summary>
		/// Initializes a new instance of the <see cref="Bmx280Base"/> class.
		/// </summary>
		/// <param name="deviceId">The ID of the device.</param>
		/// <param name="i2cDevice">The <see cref="I2cDevice"/> to create with.</param>
		protected Bmx280Base(Byte deviceId, I2cDevice i2cDevice)
			: base(deviceId, i2cDevice)
		{
		}

		/// <summary>
		/// Gets or sets the IIR filter mode.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <see cref="Bmx280FilteringMode"/> is set to an undefined mode.</exception>
		public Bmx280FilteringMode FilterMode
		{
			get => _filteringMode;
			set
			{
				var current = Read8BitsFromRegister((Byte)Bmx280Register.CONFIG);
				current = (Byte)((current & 0b_1110_0011) | (Byte)value << 2);

				Span<Byte> command = stackalloc[]
				{
					(Byte)Bmx280Register.CONFIG,
					current
				};
				_i2cDevice.Write(command);
				_filteringMode = value;
			}
		}

		/// <summary>
		/// Gets or sets the standby time between two consecutive measurements.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <see cref="Bmxx80.StandbyTime"/> is set to an undefined mode.</exception>
		public StandbyTime StandbyTime
		{
			get => _standbyTime;
			set
			{
				var current = Read8BitsFromRegister((Byte)Bmx280Register.CONFIG);
				current = (Byte)((current & 0b_0001_1111) | (Byte)value << 5);

				Span<Byte> command = stackalloc[]
				{
					(Byte)Bmx280Register.CONFIG,
					current
				};
				_i2cDevice.Write(command);
				_standbyTime = value;
			}
		}

		/// <summary>
		/// Reads the temperature. A return value indicates whether the reading succeeded.
		/// </summary>
		/// <param name="temperature">
		/// Contains the measured temperature if the <see cref="Bmxx80Base.TemperatureSampling"/> was not set to <see cref="Sampling.Skipped"/>.
		/// Contains <see cref="double.NaN"/> otherwise.
		/// </param>
		/// <returns><code>true</code> if measurement was not skipped, otherwise <code>false</code>.</returns>
		public override Boolean TryReadTemperature(out Temperature temperature)
		{
			if(TemperatureSampling == Sampling.Skipped)
			{
				temperature = Temperature.FromCelsius(Double.NaN);
				return false;
			}

			var temp = (Int32)Read24BitsFromRegister((Byte)Bmx280Register.TEMPDATA_MSB, Endianness.BigEndian);

			temperature = CompensateTemperature(temp >> 4);
			return true;
		}

		/// <summary>
		/// Read the <see cref="Bmx280PowerMode"/> state.
		/// </summary>
		/// <returns>The current <see cref="Bmx280PowerMode"/>.</returns>
		/// <exception cref="NotImplementedException">Thrown when the power mode does not match a defined mode in <see cref="Bmx280PowerMode"/>.</exception>
		public Bmx280PowerMode ReadPowerMode()
		{
			var read = Read8BitsFromRegister(_controlRegister);

			// Get only the power mode bits.
			var powerMode = (Byte)(read & 0b_0000_0011);

			if(Enum.IsDefined(typeof(Bmx280PowerMode), powerMode) == false)
			{
				throw new IOException("Read unexpected power mode");
			}

			switch(powerMode)
			{
				case 0b00:
					return Bmx280PowerMode.Sleep;
				case 0b10:
					return Bmx280PowerMode.Forced;
				case 0b11:
					return Bmx280PowerMode.Normal;
				default:
					throw new NotImplementedException($"Read power mode not defined by specification.");
			}

			//return powerMode switch
			//{
			//	0b00 => Bmx280PowerMode.Sleep,
			//	0b10 => Bmx280PowerMode.Forced,
			//	0b11 => Bmx280PowerMode.Normal,
			//	_ => throw new NotImplementedException($"Read power mode not defined by specification.")
			//};
		}

		/// <summary>
		/// Reads the pressure. A return value indicates whether the reading succeeded.
		/// </summary>
		/// <param name="pressure">
		/// Contains the measured pressure in Pa if the <see cref="Bmxx80Base.PressureSampling"/> was not set to <see cref="Sampling.Skipped"/>.
		/// Contains <see cref="double.NaN"/> otherwise.
		/// </param>
		/// <returns><code>true</code> if measurement was not skipped, otherwise <code>false</code>.</returns>
		public override Boolean TryReadPressure(out Pressure pressure)
		{
			if(PressureSampling == Sampling.Skipped)
			{
				pressure = Pressure.FromPascal(Double.NaN);
				return false;
			}

			// Read the temperature first to load the t_fine value for compensation.
			TryReadTemperature(out _);

			// Read pressure data.
			var press = (Int32)Read24BitsFromRegister((Byte)Bmx280Register.PRESSUREDATA, Endianness.BigEndian);

			// Convert the raw value to the pressure in Pa.
			var pressPa = CompensatePressure(press >> 4);

			// Return the pressure as a Pressure instance.
			pressure = Pressure.FromHectopascal(pressPa.Hectopascal / 256);
			return true;
		}

		/// <summary>
		/// Calculates the altitude in meters from the specified sea-level pressure(in hPa).
		/// </summary>
		/// <param name="seaLevelPressure">Sea-level pressure</param>
		/// <param name="altitude">
		/// Contains the calculated metres above sea-level if the <see cref="Bmxx80Base.PressureSampling"/> was not set to <see cref="Sampling.Skipped"/>.
		/// Contains <see cref="double.NaN"/> otherwise.
		/// </param>
		/// <returns><code>true</code> if pressure measurement was not skipped, otherwise <code>false</code>.</returns>
		public Boolean TryReadAltitude(Pressure seaLevelPressure, out Double altitude)
		{
			// Read the pressure first.
			var success = TryReadPressure(out var pressure);
			if(!success)
			{
				altitude = Double.NaN;
				return false;
			}

			// Calculate and return the altitude using the international barometric formula.
			altitude = 44330.0 * (1.0 - Math.Pow(pressure.Hectopascal / seaLevelPressure.Hectopascal, 0.1903));
			return true;
		}

		/// <summary>
		/// Calculates the altitude in meters from the mean sea-level pressure.
		/// </summary>
		/// <param name="altitude">
		/// Contains the calculated metres above sea-level if the <see cref="Bmxx80Base.PressureSampling"/> was not set to <see cref="Sampling.Skipped"/>.
		/// Contains <see cref="double.NaN"/> otherwise.
		/// </param>
		/// <returns><code>true</code> if pressure measurement was not skipped, otherwise <code>false</code>.</returns>
		public Boolean TryReadAltitude(out Double altitude)
		{
			return TryReadAltitude(Pressure.MeanSeaLevel, out altitude);
		}

		/// <summary>
		/// Get the current status of the device.
		/// </summary>
		/// <returns>The <see cref="DeviceStatus"/>.</returns>
		public DeviceStatus ReadStatus()
		{
			var status = Read8BitsFromRegister((Byte)Bmx280Register.STATUS);

			// Bit 3.
			var measuring = ((status >> 3) & 1) == 1;

			// Bit 0.
			var imageUpdating = (status & 1) == 1;

			return new DeviceStatus
			{
				ImageUpdating = imageUpdating,
				Measuring = measuring
			};
		}

		/// <summary>
		/// Sets the power mode to the given mode
		/// </summary>
		/// <param name="powerMode">The <see cref="Bmx280PowerMode"/> to set.</param>
		public void SetPowerMode(Bmx280PowerMode powerMode)
		{
			var read = Read8BitsFromRegister(_controlRegister);

			// Clear last 2 bits.
			var cleared = (Byte)(read & 0b_1111_1100);

			Span<Byte> command = stackalloc[]
			{
				_controlRegister,
				(Byte)(cleared | (Byte)powerMode)
			};
			_i2cDevice.Write(command);
		}

		/// <summary>
		/// Gets the required time in ms to perform a measurement with the current sampling modes.
		/// </summary>
		/// <returns>The time it takes for the chip to read data in milliseconds rounded up.</returns>
		public virtual Int32 GetMeasurementDuration()
		{
			return s_osToMeasCycles[(Int32)PressureSampling] + s_osToMeasCycles[(Int32)TemperatureSampling];
		}

		/// <summary>
		/// Sets the default configuration for the sensor.
		/// </summary>
		protected override void SetDefaultConfiguration()
		{
			base.SetDefaultConfiguration();
			FilterMode = Bmx280FilteringMode.Off;
			StandbyTime = StandbyTime.Ms125;
		}

		/// <summary>
		/// Compensates the pressure in Pa, in Q24.8 format (24 integer bits and 8 fractional bits).
		/// </summary>
		/// <param name="adcPressure">The pressure value read from the device.</param>
		/// <returns>Pressure in Hectopascals (hPa).</returns>
		/// <remarks>
		/// Output value of “24674867” represents 24674867/256 = 96386.2 Pa = 963.862 hPa.
		/// </remarks>
		private Pressure CompensatePressure(Int64 adcPressure)
		{
			// Formula from the datasheet http://www.adafruit.com/datasheets/BST-BMP280-DS001-11.pdf
			// The pressure is calculated using the compensation formula in the BMP280 datasheet
			Int64 var1 = TemperatureFine - 128000;
			var var2 = var1 * var1 * (Int64)_calibrationData.DigP6;
			var2 = var2 + ((var1 * (Int64)_calibrationData.DigP5) << 17);
			var2 = var2 + ((Int64)_calibrationData.DigP4 << 35);
			var1 = ((var1 * var1 * (Int64)_calibrationData.DigP3) >> 8) + ((var1 * (Int64)_calibrationData.DigP2) << 12);
			var1 = ((((1L << 47) + var1)) * (Int64)_calibrationData.DigP1) >> 33;
			if(var1 == 0)
			{
				return Pressure.FromPascal(0); // Avoid exception caused by division by zero
			}

			// Perform calibration operations
			var p = 1048576 - adcPressure;
			p = (((p << 31) - var2) * 3125) / var1;
			var1 = ((Int64)_calibrationData.DigP9 * (p >> 13) * (p >> 13)) >> 25;
			var2 = ((Int64)_calibrationData.DigP8 * p) >> 19;
			p = ((p + var1 + var2) >> 8) + ((Int64)_calibrationData.DigP7 << 4);

			return Pressure.FromPascal(p);
		}
	}
}
