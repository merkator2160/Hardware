using System.Diagnostics;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	public sealed class UnosquareUnit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			GetBoardInfo();
		}


		// SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
		private void GetBoardInfo()
		{
			Pi.Init<BootstrapWiringPi>();

			var info = Pi.Info;

			Debug.Write(info.ToString());
		}
	}
}