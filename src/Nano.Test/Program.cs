using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Nano.Test
{
	public class Program
	{
		private static String _ssid = "2160";
		private static String _apPassword = "zxcvbnm,";


		public static void Main()
		{
			Debug.WriteLine("Hello from nanoFramework!");

			Run();



			Thread.Sleep(Timeout.Infinite);


		}
		public static void Run()
		{
			Debug.WriteLine("Waiting for network up and IP address...");
			Boolean success;
			CancellationTokenSource cs = new(60000);

			success = NetworkHelper.ConnectWifiDhcp(_ssid, _apPassword, setDateTime: true, token: cs.Token);
			if(!success)
			{
				Debug.WriteLine($"Can't get a proper IP address and DateTime, error: {NetworkHelper.ConnectionError.Error}.");
				if(NetworkHelper.ConnectionError.Exception != null)
				{
					Debug.WriteLine($"Exception: {NetworkHelper.ConnectionError.Exception}");
				}
				return;
			}

			// get host entry for How's my SSL test site
			var hostEntry = Dns.GetHostEntry("httpbin.org");

			// need an IPEndPoint from that one above
			var ep = new IPEndPoint(hostEntry.AddressList[0], 80);

			Debug.WriteLine("Opening socket...");
			using(var mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				try
				{
					Debug.WriteLine("Connecting...");

					mySocket.Connect(ep);

					var buffer = Encoding.UTF8.GetBytes("GET / HTTP/1.0\r\n\r\n");

					mySocket.Send(buffer);

					Debug.WriteLine($"Wrote {buffer.Length} bytes");

					buffer = new Byte[1024];
					var bytes = mySocket.Receive(buffer);

					Debug.WriteLine($"Read {bytes} bytes");

					if(bytes > 0)
					{
						var result = new String(Encoding.UTF8.GetChars(buffer));
						Debug.WriteLine(result);
					}
				}
				catch(SocketException ex)
				{
					Debug.WriteLine($"** Socket exception occurred: {ex.Message} error code {ex.ErrorCode}!**");
				}
				catch(Exception ex)
				{
					Debug.WriteLine($"** Exception occurred: {ex.Message}!**");
				}
				finally
				{
					Debug.WriteLine("Closing socket");
					mySocket.Close();
				}
			}
		}



	}
}
