using Common.Drivers;
using System;
using System.Threading;

namespace Raspberry.Sandbox.Samples
{
	internal sealed class Program
	{
		public static void Main(String[] args)
		{
			using(var pwmChannel = new SoftwarePwmChannel(17, 200, 0))
			{
				pwmChannel.Start();
				for(var fill = 0.0; fill <= 1.0; fill += 0.01)
				{
					pwmChannel.DutyCycle = fill;
					Thread.Sleep(500);
				}
			}
		}
	}
}
