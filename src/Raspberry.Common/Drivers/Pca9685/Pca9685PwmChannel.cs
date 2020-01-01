using System;
using System.Device.Pwm;

namespace Common.Drivers.Pca9685
{
	internal class Pca9685PwmChannel : PwmChannel
	{
		private Pca9685Driver _parent;
		private readonly Int32 _channel;
		private Boolean _running;
		private Double _dutyCycle;


		public Pca9685PwmChannel(Pca9685Driver parent, Int32 channel)
		{
			_parent = parent;
			_channel = channel;
			_dutyCycle = ActualDutyCycle;
			_running = true;
		}


		// PROPERTIES /////////////////////////////////////////////////////////////////////////////
		public override Int32 Frequency
		{
			get => (Int32)Math.Round(_parent.PwmFrequency);
			set => throw new InvalidOperationException("Frequency can only be changed globally for all channels in the Pca9685 instance.");
		}
		private Double ActualDutyCycle
		{
			get => _parent.GetDutyCycle(_channel);
			set => _parent.SetDutyCycleInternal(_channel, value);
		}
		public override Double DutyCycle
		{
			get => _running ? ActualDutyCycle : _dutyCycle;
			set
			{
				_dutyCycle = value;

				if(_running)
				{
					ActualDutyCycle = value;
				}
			}
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public override void Start()
		{
			_running = true;
			ActualDutyCycle = _dutyCycle;
		}
		public override void Stop()
		{
			_running = false;
			ActualDutyCycle = 0.0;
		}


		// IDisposable ////////////////////////////////////////////////////////////////////////////
		protected override void Dispose(Boolean disposing)
		{
			_parent?.SetChannelAsDestroyed(_channel);
			_parent = null;
		}
	}
}
