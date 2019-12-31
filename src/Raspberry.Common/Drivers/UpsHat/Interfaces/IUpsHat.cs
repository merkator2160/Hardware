using System;

namespace Common.Drivers.UpsHat.Interfaces
{
	public interface IUpsHat
	{
		Single ReadVoltage();
		Byte ReadCapacity();
	}
}