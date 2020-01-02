﻿using Common.Helpers;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class I2cScannerUnit
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			var devices = I2cHelper.ScanBusAsync().GetAwaiter().GetResult();
			foreach(var x in devices)
			{
				Debug.WriteLine($"Device found at address: 0x{x:X}");
			}
		}
	}
}