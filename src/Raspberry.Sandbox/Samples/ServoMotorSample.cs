using Common.Drivers;
using System;
using System.Device.Pwm;

namespace Raspberry.Sandbox.Samples
{
	internal class ServoMotorSample
	{
		public static void Run()
		{
			Console.WriteLine("Hello Servo Motor!");

			using var pwmChannel = PwmChannel.Create(0, 0, 50);
			using var servoMotor = new ServoMotor(pwmChannel, 160, 700, 2200);

			WritePulseWidth(pwmChannel, servoMotor);
			// WriteAngle(pwmChannel, servoMotor);
		}
		private static void WritePulseWidth(PwmChannel pwmChannel, ServoMotor servoMotor)
		{
			servoMotor.Start();

			while(true)
			{
				Console.WriteLine("Enter a pulse width in microseconds ('Q' to quit). ");
				var pulseWidth = Console.ReadLine();

				if(pulseWidth.ToUpper() == "Q")
				{
					break;
				}

				if(!int.TryParse(pulseWidth, out var pulseWidthValue))
				{
					Console.WriteLine($"Can not parse {pulseWidth}.  Try again.");
				}

				servoMotor.WritePulseWidth(pulseWidthValue);
				Console.WriteLine($"Duty Cycle: {pwmChannel.DutyCycle * 100.0}%");
			}

			servoMotor.Stop();
		}
		private static void WriteAngle(PwmChannel pwmChannel, ServoMotor servoMotor)
		{
			servoMotor.Start();

			while(true)
			{
				Console.WriteLine("Enter an angle ('Q' to quit). ");
				var angle = Console.ReadLine();

				if(angle.ToUpper() == "Q")
				{
					break;
				}

				if(!int.TryParse(angle, out var angleValue))
				{
					Console.WriteLine($"Can not parse {angle}.  Try again.");
				}

				servoMotor.WriteAngle(angleValue);
				Console.WriteLine($"Duty Cycle: {pwmChannel.DutyCycle * 100.0}%");
			}

			servoMotor.Stop();
		}
	}
}
