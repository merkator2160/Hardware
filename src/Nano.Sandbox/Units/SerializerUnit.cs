using Nano.Common.Models;
using nanoFramework.Json;
using System;
using System.Diagnostics;

namespace Nano.Sandbox.Units
{
	internal static class SerializerUnit
	{
		public static void Run()
		{
			var configuration = new Configuration()
			{
				SerialNumber = Guid.NewGuid().ToString()
			};
			var jsonStr = JsonConvert.SerializeObject(configuration);
			Debug.WriteLine(jsonStr);
		}
	}
}