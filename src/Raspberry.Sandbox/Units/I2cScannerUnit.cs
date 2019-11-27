using Common.Helpers;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class I2cScannerUnit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			var devices = I2cScanner.FindDevicesAsync().GetAwaiter().GetResult();
		}
	}
}