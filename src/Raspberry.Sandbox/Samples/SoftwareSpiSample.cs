using Common.Drivers;

namespace Raspberry.Sandbox.Samples
{
	internal sealed class SoftwareSpiSample
	{
		public static void Run()
		{
			using(var spi = new SoftwareSpi(clk: 6, miso: 23, mosi: 5, cs: 24))
			{
				// do stuff over SPI
			}
		}
	}
}