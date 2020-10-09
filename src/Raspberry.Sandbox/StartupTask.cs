using Raspberry.Sandbox.Units;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox
{
	public sealed class StartupTask : IBackgroundTask
	{
		private BackgroundTaskDeferral deferral;


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			deferral = taskInstance.GetDeferral();

			new Ks0218HatUnit().Run(taskInstance);
			//new Bmp280Unit().Run(taskInstance);
			//new I2cScannerUnit().Run(taskInstance);

			deferral.Complete();
		}
	}
}