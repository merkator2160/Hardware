using MqttMessageProcessor.Models.Config;
using System;

namespace MqttMessageProcessor
{
	class Program
	{
		static void Main(String[] args)
		{
			try
			{
				using(var processor = new Processor(new ProcessorConfig()
				{
					IpAddress = "192.168.1.11",
					Port = 1183,
					Login = "",
					Password = "",
				}))
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