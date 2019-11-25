using Microsoft.IoT.Lightning.Providers;
using Raspberry.Sandbox.Units;
using Windows.ApplicationModel.Background;
using Windows.Devices;

namespace Raspberry.Sandbox
{
	public sealed class StartupTask : IBackgroundTask
	{
		private BackgroundTaskDeferral deferral;


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			deferral = taskInstance.GetDeferral();

			if(LightningProvider.IsLightningEnabled)
				LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();

			new I2cScannerUnite().Run(taskInstance);

			deferral.Complete();
		}
	}
}