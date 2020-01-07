using Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Device.I2c;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace Raspberry.Sandbox.Units
{
	internal sealed class I2cListenerUnit
	{
		private const Int32 _arduinoAddress = 0x08;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			using(var device = I2cDevice.Create(new I2cConnectionSettings(1, _arduinoAddress)))
			{
				while(true)
				{
					var values = ReadValues(device);
					var str = JsonConvert.SerializeObject(values);

					Debug.WriteLine(str);
				}
			}
		}


		// SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
		private static Int16[] ReadValues(I2cDevice pwmDevice)
		{
			Span<Byte> inputBuffer = stackalloc Byte[18];

			pwmDevice.Read(inputBuffer);

			return inputBuffer.ToInt16Array();
		}
	}
}