using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.System.Threading;

namespace Raspberry.Sandbox.Units
{
	public sealed class BlinkyUnit
	{
		private BackgroundTaskDeferral deferral;
		private GpioPinValue value = GpioPinValue.High;
		private GpioPin _ledPin;
		private ThreadPoolTimer _timer;

		private volatile Boolean _cancelRequested;




		// HANDLERS ///////////////////////////////////////////////////////////////////////////////
		private void OnTimerTick(ThreadPoolTimer timer)
		{
			if(_cancelRequested)
			{
				timer.Cancel();
				deferral.Complete();

				return;
			}

			value = value == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High;
			_ledPin.Write(value);
		}
		private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			_cancelRequested = true;
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			taskInstance.Canceled += OnCanceled;

			deferral = taskInstance.GetDeferral();

			InitPins();

			_timer = ThreadPoolTimer.CreatePeriodicTimer(OnTimerTick, TimeSpan.FromMilliseconds(500));
		}
		private void InitPins()
		{
			_ledPin = GpioController.GetDefault().OpenPin(4);
			_ledPin.Write(GpioPinValue.High);
			_ledPin.SetDriveMode(GpioPinDriveMode.Output);
		}
	}
}