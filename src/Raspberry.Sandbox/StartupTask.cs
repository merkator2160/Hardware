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

			new I2cEchoUnit().Run(taskInstance);

			deferral.Complete();
		}
	}
}