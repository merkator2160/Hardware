using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.WiFi;

namespace Nano.Common.Helpers
{
	public static class WifiHelper
	{
		private const String _ssid = "2160";
		private const String _apPassword = "zxcvbnm,";

		private const String _deviceIpAddress = "192.168.1.50";
		private const String _subnetMask = "255.255.255.0";
		private const String _gatewayAddress = "192.168.1.2";
		private const String _dns1 = "8.8.8.8";
		private const String _dns2 = "8.8.4.4";

		private static Boolean _connectedSuccessfully;



		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public static Boolean TryConnectNetworkWithHelper()
		{
			Debug.WriteLine("Waiting for network up...");

			var cancellationTokenSource = new CancellationTokenSource(60000);
			var ipConfiguration = new IPConfiguration(_deviceIpAddress, _subnetMask, _gatewayAddress, new String[] { _dns1, _dns2 });
			var success = NetworkHelper.ConnectWifiFixAddress(_ssid, _apPassword, ipConfiguration, setDateTime: true, token: cancellationTokenSource.Token);
			//var success = NetworkHelper.ConnectWifiDhcp(_ssid, _apPassword, setDateTime: true, token: cancellationTokenSource.Token);
			if(success)
				return true;

			Debug.WriteLine($"Can't get a proper IP address and DateTime, error: {NetworkHelper.ConnectionError.Error}");
			if(NetworkHelper.ConnectionError.Exception != null)
				Debug.WriteLine($"Exception: {NetworkHelper.ConnectionError.Exception}");

			return false;
		}
		public static Boolean TryConnectNetwork()
		{
			try
			{
				var wifi = WiFiAdapter.FindAllAdapters()[0];
				wifi.AvailableNetworksChanged += WiFiAvailableNetworksChanged;

				Debug.WriteLine("Starting WiFi scan...");
				wifi.ScanAsync();

				while(!_connectedSuccessfully)
				{
					Thread.Sleep(1000);
				}

				return true;
			}
			catch(Exception ex)
			{
				Debug.WriteLine("Message:" + ex.Message);
				Debug.WriteLine("Stack trace:" + ex.StackTrace);

				return false;
			}
		}
		private static void WiFiAvailableNetworksChanged(WiFiAdapter adapter, Object e)
		{
			Debug.WriteLine("Report:");

			foreach(var net in adapter.NetworkReport.AvailableNetworks)
			{
				Debug.WriteLine($"Net SSID :{net.Ssid}, BSSID: {net.Bsid}, rssi: {net.NetworkRssiInDecibelMilliwatts}, signal: {net.SignalBars}");

				if(net.Ssid == _ssid)
				{
					adapter.Disconnect();   // Disconnect in case we are already connected

					var result = adapter.Connect(net, WiFiReconnectionKind.Automatic, _apPassword);
					if(result.ConnectionStatus == WiFiConnectionStatus.Success)
					{
						Debug.WriteLine("Connected to Wifi network");
						_connectedSuccessfully = true;
					}
					else
					{
						Debug.WriteLine($"Error {result.ConnectionStatus} connecting to Wifi network");
					}
				}
			}
		}
	}
}