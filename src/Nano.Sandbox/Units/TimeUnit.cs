using System;
using System.Diagnostics;

namespace Nano.Sandbox.Units
{
	internal static class TimeUnit
	{
		public static void Run()
		{
			var dateTime = DateTime.UtcNow;

			Debug.WriteLine(dateTime.ToString("G"));
		}
	}
}