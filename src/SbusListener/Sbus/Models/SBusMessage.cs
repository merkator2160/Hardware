using System;

namespace SbusListener.Sbus.Models
{
	public class SBusMessage
	{
		public UInt16[] ServoChannels { get; set; }
		public Boolean[] DigitalChannels { get; set; }
		public Boolean FailSafe { get; set; }
		public Boolean IsFrameLost { get; set; }
	}
}