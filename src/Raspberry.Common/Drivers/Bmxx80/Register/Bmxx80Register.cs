
namespace Common.Drivers.Bmxx80.Register
{
	/// <summary>
	/// Registers shared in the Bmxx80 family.
	/// </summary>
	internal enum Bmxx80Register : byte
	{
		CHIPID = 0xD0,
		RESET = 0xE0
	}
}
