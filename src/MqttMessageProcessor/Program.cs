using MqttMessageProcessor.Services.Models.Config;
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
					HostName = "192.168.1.11",
					Port = 1883,
					Login = "user",
					Password = ";4,wnmCvQ.7hMDdWuqv*",
					ClientId = "63345ac1-5260-4a84-b4cc-e45f5ff9e388"
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