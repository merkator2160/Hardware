using Core.Sandbox.Units.SbusSoftwareDecoder.Sbus.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace Core.Sandbox.Units.SbusSoftwareDecoder.Sbus
{
	/// <summary>
	/// https://github.com/bolderflight/SBUS
	/// </summary>
	public class SbusConverter
	{
		private const Int32 _messageLength = 25;
		private const Int32 _endOfStream = -1;
		private const Byte _sBusMessageHeader = 0x1f;
		private const Byte _sBusMessageEndByte = 0x00;

		private readonly KalmanFilter[] _filter;


		public SbusConverter()
		{
			_filter = CreateFilter(12, 585.0f, 10.0f);  // filter size must be equals the number of channels
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		public Boolean IsFilterEnable { get; set; }
		public Boolean IgnoreLostFrame { get; set; }


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public Boolean TryReadMessage(SerialPort serialPort, out Byte[] messageBuffer)
		{
			messageBuffer = new Byte[_messageLength];

			if(serialPort.BytesToRead < _messageLength)
				return false;

			do
			{
				var value = serialPort.ReadByte();
				if(value == _endOfStream)
					return false;

				if(value == _sBusMessageHeader)
				{
					messageBuffer[0] = (Byte)value;
					for(var i = 1; i < _messageLength; i++)
					{
						messageBuffer[i] = (Byte)serialPort.ReadByte();
					}

					return ValidateMessage(messageBuffer);
				}

			} while(serialPort.BytesToRead > 0);

			return false;
		}
		private Boolean ValidateMessage(Byte[] messageBuffer)
		{
			if(messageBuffer[0] != _sBusMessageHeader)
				return false;

			if(messageBuffer[24] != _sBusMessageEndByte)
				return false;

			if(IgnoreLostFrame && GetLostFrame(messageBuffer))
				return false;

			return true;
		}


		// MESSAGE PARSER FUNCTIONS ///////////////////////////////////////////////////////////////
		/// <summary>
		/// Byte[0]: SBUS Header, 0x1F
		/// Byte[1-22]: 16 servo channels, 11 bits per servo channel
		/// Byte[23]:
		///		Bit 7: digital channel 17 (0x80)
		///		Bit 6: digital channel 18 (0x40)
		///		Bit 5: frame lost (0x20)
		///		Bit 4: FailSafe activated (0x10)
		///		Bit 0 - 3: n/a
		/// Byte[24]: SBUS End Byte, 0x00
		/// </summary>
		public SBusMessage Parse(Byte[] sbusData)
		{
			return new SBusMessage()
			{
				ServoChannels = GetServoValues(sbusData),
				DigitalChannels = GetDigitalChannelValues(sbusData),
				FailSafe = GetFailSafeValue(sbusData),
				IsFrameLost = GetLostFrame(sbusData)
			};
		}
		private UInt16[] GetServoValues(Byte[] sbusData)
		{
			var values = new UInt16[]
			{
				(UInt16)((sbusData[1] | sbusData[2] << 8) & 0x7FF),
				(UInt16)((sbusData[2] >> 3 | sbusData[3] << 5) & 0x7FF),
				(UInt16)((sbusData[3] >> 6 | sbusData[4] << 2 | sbusData[5] << 10) & 0x7FF),
				(UInt16)((sbusData[5] >> 1 | sbusData[6] << 7) & 0x07FF),

				(UInt16)((sbusData[6] >> 4 | sbusData[7] << 4) & 0x07FF),
				(UInt16)((sbusData[7] >> 7 | sbusData[8] << 1 | sbusData[9] << 9) & 0x07FF),
				(UInt16)((sbusData[9] >> 2 | sbusData[10] << 6) & 0x07FF),
				(UInt16)((sbusData[10] >> 5 | sbusData[11] << 3) & 0x07FF),

				(UInt16)((sbusData[12] | sbusData[13] << 8) & 0x07FF),
				(UInt16)((sbusData[13] >> 3 | sbusData[14] << 5) & 0x07FF),
				(UInt16)((sbusData[14] >> 6 | sbusData[15] << 2 | sbusData[16] << 10) & 0x07FF),
				(UInt16)((sbusData[16] >> 1 | sbusData[17] << 7) & 0x07FF),

				// My RadioLink AT9S supports only 12 channels
				//(UInt16)((sbusData[17] >> 4 | sbusData[18] << 4) & 0x07FF),
				//(UInt16)((sbusData[18] >> 7 | sbusData[19] << 1 | sbusData[20] << 9) & 0x07FF),
				//(UInt16)((sbusData[20] >> 2 | sbusData[21] << 6) & 0x07FF),
				//(UInt16)((sbusData[21] >> 5 | sbusData[22] << 3) & 0x07FF)
			};

			if(IsFilterEnable)
				return ApplyFilter(values);

			return values;
		}
		private Boolean[] GetDigitalChannelValues(Byte[] sbusData)
		{
			return new Boolean[]
			{
				(sbusData[23] & (1 << 7)) == 0x80,
				(sbusData[23] & (1 << 6)) == 0x40
			};
		}
		private Boolean GetFailSafeValue(Byte[] sbusData)
		{
			return (sbusData[23] & (1 << 4)) == 0x10;
		}
		private Boolean GetLostFrame(Byte[] sbusData)
		{
			return (sbusData[23] & (1 << 5)) == 0x20;
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private KalmanFilter[] CreateFilter(Int32 filterSize, Single averageDeviation, Single reactionSpeed)
		{
			var filterList = new List<KalmanFilter>(filterSize);
			for(var i = 0; i < filterSize; i++)
			{
				filterList.Add(new KalmanFilter(averageDeviation, reactionSpeed));
			}

			return filterList.ToArray();
		}
		private UInt16[] ApplyFilter(UInt16[] values)
		{
			return values.Select((x, i) => (UInt16)_filter[i].Calculate(x)).ToArray();
		}
	}
}