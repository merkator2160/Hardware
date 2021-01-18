using System;

namespace MqttMessageProcessor
{
	class Program
	{
		private const String _brokerIpAddress = "192.168.1.11";     // TODO: Move to config file


		static void Main(String[] args)
		{
			try
			{
				using(var processor = new Processor(_brokerIpAddress))
				{
					processor.Start();

					Console.ReadKey();
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.ReadKey();
			}
		}
	}
}