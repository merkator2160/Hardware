using Nano.Common.Helpers;
using Nano.Common.Models;
using System;
using System.Diagnostics;

namespace Nano.Sandbox.Units
{
	internal static class StorageUnit
	{
		public static void Run()
		{
			var store = new ConfigurationStore();
			var configuration = new Configuration()
			{
				SerialNumber = Guid.NewGuid().ToString()
			};

			store.SaveConfig(configuration);
			var config = store.GetConfig();

			Debug.WriteLine(config.SerialNumber);
		}
	}
}