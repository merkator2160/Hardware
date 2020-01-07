using Common.Const.Units;
using Common.Drivers.Bmxx80.CalibrationData;
using Common.Drivers.Bmxx80.Register;
using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.IO;

namespace Common.Drivers.Bmxx80
{
	/// <summary>
	/// Represents the core functionality of the Bmxx80 family.
	/// </summary>
	public abstract class Bmxx80Base : IDisposable
	{
		internal Bmxx80CalibrationData _calibrationData;
		protected I2cDevice _i2cDevice;
		protected CommunicationProtocol _communicationProtocol;
		protected Byte _controlRegister;

		private Sampling _temperatureSampling;
		private Sampling _pressureSampling;



		protected Bmxx80Base(Byte deviceId, I2cDevice i2cDevice)
		{
			_i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));
			_i2cDevice.WriteByte((Byte)Bmxx80Register.CHIPID);

			var readSignature = _i2cDevice.ReadByte();
			if(readSignature != deviceId)
				throw new IOException($"Unable to find a chip with id {deviceId}");

			ReadCalibrationData();
			SetDefaultConfiguration();
		}



		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets or sets the pressure sampling.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <see cref="Sampling"/> is set to an undefined mode.</exception>
		public Sampling PressureSampling
		{
			get => _pressureSampling;
			set
			{
				var status = Read8BitsFromRegister(_controlRegister);
				status = (Byte)(status & 0b1110_0011);
				status = (Byte)(status | (Byte)value << 2);

				Span<Byte> command = stackalloc[]
				{
					_controlRegister,
					status
				};
				_i2cDevice.Write(command);
				_pressureSampling = value;
			}
		}

		/// <summary>
		/// Gets or sets the temperature sampling.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <see cref="Sampling"/> is set to an undefined mode.</exception>
		public Sampling TemperatureSampling
		{
			get => _temperatureSampling;
			set
			{
				var status = Read8BitsFromRegister(_controlRegister);
				status = (Byte)(status & 0b0001_1111);
				status = (Byte)(status | (Byte)value << 5);

				Span<Byte> command = stackalloc[]
				{
					_controlRegister,
					status
				};
				_i2cDevice.Write(command);
				_temperatureSampling = value;
			}
		}

		/// <summary>
		/// The variable TemperatureFine carries a fine resolution temperature value over to the
		/// pressure compensation formula and could be implemented as a global variable.
		/// </summary>
		protected Int32 TemperatureFine
		{
			get;
			set;
		}

		/// <summary>
		/// The temperature calibration factor.
		/// </summary>
		protected virtual Int32 TempCalibrationFactor => 1;


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// When called, the device is reset using the complete power-on-reset procedure.
		/// The device will reset to the default configuration.
		/// </summary>
		public void Reset()
		{
			const Byte resetCommand = 0xB6;
			Span<Byte> command = stackalloc[]
			{
				(Byte)Bmxx80Register.RESET,
				resetCommand
			};
			_i2cDevice.Write(command);

			SetDefaultConfiguration();
		}

		/// <summary>
		/// Reads the temperature. A return value indicates whether the reading succeeded.
		/// </summary>
		/// <param name="temperature">
		/// Contains the measured temperature if the <see cref="TemperatureSampling"/> was not set to <see cref="Sampling.Skipped"/>.
		/// Contains <see cref="double.NaN"/> otherwise.
		/// </param>
		/// <returns><code>true</code> if measurement was not skipped, otherwise <code>false</code>.</returns>
		public abstract Boolean TryReadTemperature(out Temperature temperature);

		/// <summary>
		/// Reads the pressure. A return value indicates whether the reading succeeded.
		/// </summary>
		/// <param name="pressure">
		/// Contains the measured pressure if the <see cref="PressureSampling"/> was not set to <see cref="Sampling.Skipped"/>.
		/// Contains <see cref="double.NaN"/> otherwise.
		/// </param>
		/// <returns><code>true</code> if measurement was not skipped, otherwise <code>false</code>.</returns>
		public abstract Boolean TryReadPressure(out Pressure pressure);


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		protected virtual void SetDefaultConfiguration()
		{
			PressureSampling = Sampling.UltraLowPower;
			TemperatureSampling = Sampling.UltraLowPower;
		}
		private void ReadCalibrationData()
		{
			switch(this)
			{
				case Bme280Driver _:
					_calibrationData = new Bme280CalibrationData();
					_controlRegister = (Byte)Bmx280Register.CTRL_MEAS;
					break;
				case Bmp280Driver _:
					_calibrationData = new Bmp280CalibrationData();
					_controlRegister = (Byte)Bmx280Register.CTRL_MEAS;
					break;
				case Bme680Driver _:
					_calibrationData = new Bme680CalibrationData();
					_controlRegister = (Byte)Bme680Register.CTRL_MEAS;
					break;
			}

			_calibrationData.ReadFromDevice(this);
		}
		/// <summary>
		/// Compensates the temperature.
		/// </summary>
		/// <param name="adcTemperature">The temperature value read from the device.</param>
		/// <returns>The <see cref="Temperature"/>.</returns>
		protected Temperature CompensateTemperature(Int32 adcTemperature)
		{
			// The temperature is calculated using the compensation formula in the BMP280 datasheet.
			// See: https://cdn-shop.adafruit.com/datasheets/BST-BMP280-DS001-11.pdf
			var var1 = ((adcTemperature / 16384.0) - (_calibrationData.DigT1 / 1024.0)) * _calibrationData.DigT2;
			var var2 = (adcTemperature / 131072.0) - (_calibrationData.DigT1 / 8192.0);
			var2 *= var2 * _calibrationData.DigT3 * TempCalibrationFactor;

			TemperatureFine = (Int32)(var1 + var2);

			var temp = (var1 + var2) / 5120.0;
			return Temperature.FromCelsius(temp);
		}

		/// <summary>
		/// Reads an 8 bit value from a register.
		/// </summary>
		/// <param name="register">Register to read from.</param>
		/// <returns>Value from register.</returns>
		protected internal Byte Read8BitsFromRegister(Byte register)
		{
			if(_communicationProtocol == CommunicationProtocol.I2c)
			{
				_i2cDevice.WriteByte(register);
				var value = _i2cDevice.ReadByte();
				return value;
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Reads a 16 bit value over I2C.
		/// </summary>
		/// <param name="register">Register to read from.</param>
		/// <param name="endianness">Interpretation of the bytes (big or little endian).</param>
		/// <returns>Value from register.</returns>
		protected internal UInt16 Read16BitsFromRegister(Byte register, Endianness endianness = Endianness.LittleEndian)
		{
			Span<Byte> bytes = stackalloc Byte[2];
			switch(_communicationProtocol)
			{
				case CommunicationProtocol.I2c:
					_i2cDevice.WriteByte(register);
					_i2cDevice.Read(bytes);
					break;
				default:
					throw new NotImplementedException();
			}

			switch(endianness)
			{
				case Endianness.LittleEndian:
					return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
				case Endianness.BigEndian:
					return BinaryPrimitives.ReadUInt16BigEndian(bytes);

				default:
					throw new ArgumentOutOfRangeException(nameof(endianness), endianness, null);
			}
		}

		/// <summary>
		/// Reads a 24 bit value over I2C.
		/// </summary>
		/// <param name="register">Register to read from.</param>
		/// <param name="endianness">Interpretation of the bytes (big or little endian).</param>
		/// <returns>Value from register.</returns>
		protected internal UInt32 Read24BitsFromRegister(Byte register, Endianness endianness = Endianness.LittleEndian)
		{
			Span<Byte> bytes = stackalloc Byte[4];
			switch(_communicationProtocol)
			{
				case CommunicationProtocol.I2c:
					_i2cDevice.WriteByte(register);
					_i2cDevice.Read(bytes.Slice(1));
					break;
				default:
					throw new NotImplementedException();
			}

			switch(endianness)
			{
				case Endianness.LittleEndian:
					return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
				case Endianness.BigEndian:
					return BinaryPrimitives.ReadUInt32BigEndian(bytes);

				default:
					throw new ArgumentOutOfRangeException(nameof(endianness), endianness, null);
			}
		}

		/// <summary>
		/// Converts byte to <see cref="Sampling"/>.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <returns><see cref="Sampling"/></returns>
		protected Sampling ByteToSampling(Byte value)
		{
			// Values >=5 equals UltraHighResolution.
			if(value >= 5)
			{
				return Sampling.UltraHighResolution;
			}

			return (Sampling)value;
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Cleanup.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the Bmxx80 and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(Boolean disposing)
		{
			_i2cDevice?.Dispose();
			_i2cDevice = null;
		}


		// ENUMS //////////////////////////////////////////////////////////////////////////////////
		protected internal enum Endianness
		{
			LittleEndian,
			BigEndian
		}
		public enum CommunicationProtocol
		{
			I2c
		}
	}
}
