using Nano.Sandbox.Units;
using System.Threading;

namespace Nano.Sandbox
{
	public class Program
	{
		public static void Main()
		{
			//if(!WifiHelper.TryConnectNetwork())
			//{
			//	Debug.WriteLine("Network connection failed!");
			//	return;
			//}

			MqttUnit.Run();

			Thread.Sleep(Timeout.Infinite);
		}
	}
}