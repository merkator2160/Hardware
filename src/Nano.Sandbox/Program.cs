using Nano.Sandbox.Units;
using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Threading;

namespace Nano.Sandbox
{
	public class Program
	{
		private const String _ssid = "2160";
		private const String _apPassword = "zxcvbnm,";

		private const String _deviceIpAddress = "192.168.1.50";
		private const String _subnetMask = "255.255.255.0";
		private const String _gatewayAddress = "192.168.1.2";
		private const String _dns1 = "8.8.8.8";
		private const String _dns2 = "8.8.4.4";


		public static void Main()
		{
			if(!TryConnectNetwork())
			{
				Thread.Sleep(5000);
				return;
			}

			HttpUnit.Run();

			//Thread.Sleep(Timeout.Infinite);
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public static Boolean TryConnectNetwork()
		{
			Debug.WriteLine("Waiting for network up...");

			var cancellationTokenSource = new CancellationTokenSource(60000);
			var ipConfiguration = new IPConfiguration(_deviceIpAddress, _subnetMask, _gatewayAddress, new String[] { _dns1, _dns2 });
			//var success = NetworkHelper.ConnectWifiFixAddress(_ssid, _apPassword, ipConfiguration, setDateTime: true, token: cancellationTokenSource.Token);
			var success = NetworkHelper.ConnectWifiDhcp(_ssid, _apPassword, setDateTime: true, token: cancellationTokenSource.Token);
			if(success)
				return true;

			Debug.WriteLine($"Can't get a proper IP address and DateTime, error: {NetworkHelper.ConnectionError.Error}");
			if(NetworkHelper.ConnectionError.Exception != null)
				Debug.WriteLine($"Exception: {NetworkHelper.ConnectionError.Exception}");

			return false;
		}
	}
}