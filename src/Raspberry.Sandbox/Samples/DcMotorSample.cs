using Common.Drivers.DcMotor;
using System;
using System.Diagnostics;
using System.Threading;

namespace Raspberry.Sandbox.Samples
{
	internal sealed class DcMotorSample
	{
		public static void Main(String[] args)
		{
			const Double Period = 10.0;
			var sw = Stopwatch.StartNew();
			// 1 pin mode
			// using (DCMotor motor = DCMotor.Create(6))
			// using (DCMotor motor = DCMotor.Create(PwmChannel.Create(0, 0, frequency: 50)))
			// 2 pin mode
			// using (DCMotor motor = DCMotor.Create(27, 22))
			// using (DCMotor motor = DCMotor.Create(new SoftwarePwmChannel(27, frequency: 50), 22))
			// 3 pin mode
			// using (DCMotor motor = DCMotor.Create(PwmChannel.Create(0, 0, frequency: 50), 23, 24))
			using(var motor = DcMotorBase.Create(6, 27, 22))
			{
				var done = false;
				Console.CancelKeyPress += (o, e) =>
				{
					done = true;
					e.Cancel = true;
				};

				String lastSpeedDisp = null;
				while(!done)
				{
					var time = sw.ElapsedMilliseconds / 1000.0;

					// Note: range is from -1 .. 1 (for 1 pin setup 0 .. 1)
					motor.Speed = Math.Sin(2.0 * Math.PI * time / Period);
					var disp = $"Speed = {motor.Speed:0.00}";
					if(disp != lastSpeedDisp)
					{
						lastSpeedDisp = disp;
						Console.WriteLine(disp);
					}

					Thread.Sleep(1);
				}
			}
		}
	}
}
