using System;

namespace Core.Sandbox.Units.SbusHardwareDecoder.Models
{
	public class SBusMessage
	{
		public UInt16[] ServoChannels { get; set; }
		public String FailSafe { get; set; }
		public String FrameLost { get; set; }
	}
}