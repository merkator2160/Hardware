using Microsoft.IoT.Lightning.Providers;
using Sandbox.Units;
using Windows.ApplicationModel.Background;
using Windows.Devices;

namespace Sandbox
{
	public sealed class StartupTask : IBackgroundTask
	{
		private BackgroundTaskDeferral deferral;


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			deferral = taskInstance.GetDeferral();

			if(LightningProvider.IsLightningEnabled)
			{
				LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
			}

			new LepinServoUnit().Run(taskInstance);

			deferral.Complete();
		}
	}
}