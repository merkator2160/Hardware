using Common.Drivers;
using Common.Drivers.Pca9685;
using System;
using System.Device.I2c;

namespace Raspberry.Sandbox.Samples
{
	public sealed class Pca9685Sample
	{
		// Some SG90s can do 180 angle range but some other will be oscillating on the edge values
		// Max angle which doesn't cause any issues found experimentally was as below.
		// The ones which can do 180 will have the minimum pulse width at around 520uS.
		private const Int32 AngleRange = 173;
		private const Int32 MinPulseWidthMicroseconds = 600;
		private const Int32 MaxPulseWidthMicroseconds = 2590;


		public static void Run()
		{
			var busId = 1;
			var selectedI2cAddress = 0b000000; // A5 A4 A3 A2 A1 A0
			var deviceAddress = 0x40 + selectedI2cAddress;

			var settings = new I2cConnectionSettings(busId, deviceAddress);
			var device = I2cDevice.Create(settings);

			using(var pca9685 = new Pca9685Driver(deviceAddress))
			{
				Console.WriteLine($"PCA9685 is ready on I2C bus {device.ConnectionSettings.BusId} with address {device.ConnectionSettings.DeviceAddress}");
				Console.WriteLine($"PWM Frequency: {pca9685.PwmFrequency}Hz");
				Console.WriteLine();
				PrintHelp();

				while(true)
				{
					var command = Console.ReadLine().ToLower().Split(' ');
					if(String.IsNullOrEmpty(command[0]))
						return;

					switch(command[0][0])
					{
						case 'q':
							pca9685.SetDutyCycleAllChannels(0.0);
							return;
						case 'f':
							{
								var freq = Double.Parse(command[1]);
								pca9685.PwmFrequency = freq;
								Console.WriteLine($"PWM Frequency has been set to {pca9685.PwmFrequency}Hz");
								break;
							}

						case 'd':
							{
								switch(command.Length)
								{
									case 2:
										{
											var value = Double.Parse(command[1]);
											pca9685.SetDutyCycleAllChannels(value);
											Console.WriteLine($"PWM duty cycle has been set to {value}");
											break;
										}

									case 3:
										{
											var channel = Int32.Parse(command[1]);
											var value = Double.Parse(command[2]);
											pca9685.SetDutyCycle(channel, value);
											Console.WriteLine($"PWM duty cycle has been set to {value}");
											break;
										}
								}

								break;
							}

						case 'h':
							{
								PrintHelp();
								break;
							}

						case 't':
							{
								var channel = Int32.Parse(command[1]);
								ServoDemo(pca9685, channel);
								PrintHelp();
								break;
							}
					}
				}
			}
		}
		private static ServoMotor CreateServo(Pca9685Driver pca9685, Int32 channel)
		{
			return new ServoMotor(pca9685.CreatePwmChannel(channel), AngleRange, MinPulseWidthMicroseconds, MaxPulseWidthMicroseconds);
		}
		private static void PrintServoDemoHelp()
		{
			Console.WriteLine("Q                   return to previous menu");
			Console.WriteLine("C                   calibrate");
			Console.WriteLine("{angle}             set angle (0 - 180)");
			Console.WriteLine();
		}
		private static void ServoDemo(Pca9685Driver pca9685, Int32 channel)
		{
			using(var servo = CreateServo(pca9685, channel))
			{
				PrintServoDemoHelp();

				while(true)
				{
					var command = Console.ReadLine().ToLower();
					if(String.IsNullOrEmpty(command) || command[0] == 'q')
					{
						return;
					}

					if(command[0] == 'c')
					{
						CalibrateServo(servo);
						PrintServoDemoHelp();
					}
					else
					{
						var value = Double.Parse(command);
						servo.WriteAngle(value);
						Console.WriteLine($"Angle set to {value}. PWM duty cycle = {pca9685.GetDutyCycle(channel)}");
					}
				}
			}
		}
		private static void CalibrateServo(ServoMotor servo)
		{
			var maximumAngle = 180;
			var minimumPulseWidthMicroseconds = 520;
			var maximumPulseWidthMicroseconds = 2590;

			Console.WriteLine("Searching for minimum pulse width");
			CalibratePulseWidth(servo, ref minimumPulseWidthMicroseconds);
			Console.WriteLine();

			Console.WriteLine("Searching for maximum pulse width");
			CalibratePulseWidth(servo, ref maximumPulseWidthMicroseconds);

			Console.WriteLine("Searching for angle range");
			Console.WriteLine("What is the angle range? (type integer with your angle range or enter to move to MIN/MAX)");

			while(true)
			{
				servo.WritePulseWidth(maximumPulseWidthMicroseconds);
				Console.WriteLine("Servo is now at MAX");
				if(Int32.TryParse(Console.ReadLine(), out maximumAngle))
				{
					break;
				}

				servo.WritePulseWidth(minimumPulseWidthMicroseconds);
				Console.WriteLine("Servo is now at MIN");

				if(Int32.TryParse(Console.ReadLine(), out maximumAngle))
				{
					break;
				}
			}

			servo.Calibrate(maximumAngle, minimumPulseWidthMicroseconds, maximumPulseWidthMicroseconds);
			Console.WriteLine($"Angle range: {maximumAngle}");
			Console.WriteLine($"Min PW [uS]: {minimumPulseWidthMicroseconds}");
			Console.WriteLine($"Max PW [uS]: {maximumPulseWidthMicroseconds}");
		}
		private static void CalibratePulseWidth(ServoMotor servo, ref Int32 pulseWidthMicroSeconds)
		{
			void SetPulseWidth(ref Int32 pulseWidth)
			{
				pulseWidth = Math.Max(pulseWidth, 0);
				servo.WritePulseWidth(pulseWidth);
			}

			Console.WriteLine("Use A/Z (1x); S/X (10x); D/C (100x)");
			Console.WriteLine("Press enter to accept value");

			while(true)
			{
				SetPulseWidth(ref pulseWidthMicroSeconds);
				Console.WriteLine($"Current value: {pulseWidthMicroSeconds}");

				switch(Console.ReadKey().Key)
				{
					case ConsoleKey.A:
						pulseWidthMicroSeconds++;
						break;
					case ConsoleKey.Z:
						pulseWidthMicroSeconds--;
						break;
					case ConsoleKey.S:
						pulseWidthMicroSeconds += 10;
						break;
					case ConsoleKey.X:
						pulseWidthMicroSeconds -= 10;
						break;
					case ConsoleKey.D:
						pulseWidthMicroSeconds += 100;
						break;
					case ConsoleKey.C:
						pulseWidthMicroSeconds -= 100;
						break;
					case ConsoleKey.Enter:
						return;
				}
			}
		}
		private static void PrintHelp()
		{
			Console.WriteLine("Command:");
			Console.WriteLine("    F {freq_hz}          set PWM frequency (Hz)");
			Console.WriteLine("    D {value}            set duty cycle (0.0 .. 1.0) for all channels");
			Console.WriteLine("    S {channel} {value}  set duty cycle (0.0 .. 1.0) for specific channel");
			Console.WriteLine("    T {channel}          servo test (defaults to SG90 params)");
			Console.WriteLine("    H                    prints help");
			Console.WriteLine("    Q                    quit");
			Console.WriteLine();
		}
	}
}
