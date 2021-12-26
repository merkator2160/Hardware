using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.WiFi;

namespace Nano.Sandbox.Units
{
	internal static class WiFiScannerUnit
	{
		public static void Run()
		{
			try
			{
				var wifi = WiFiAdapter.FindAllAdapters()[0];
				wifi.AvailableNetworksChanged += WiFiAvailableNetworksChanged;

				while(true)
				{
					Debug.WriteLine("Starting WiFi scan...");
					wifi.ScanAsync();

					Thread.Sleep(10000);
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine("Message:" + ex.Message);
				Debug.WriteLine("Stack trace:" + ex.StackTrace);
			}
		}
		private static void WiFiAvailableNetworksChanged(WiFiAdapter adapter, Object e)
		{
			Debug.WriteLine("Report:");

			foreach(var net in adapter.NetworkReport.AvailableNetworks)
			{
				Debug.WriteLine($"Net SSID :{net.Ssid}, BSSID: {net.Bsid}, rssi: {net.NetworkRssiInDecibelMilliwatts}, signal: {net.SignalBars}");
			}
		}
	}
}