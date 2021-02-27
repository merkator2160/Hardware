using Autofac;
using IotHub.Common.Config;
using IotHub.Common.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MqttMessageProcessor.Services.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;

namespace MqttMessageProcessor
{
	class Program
	{
		static void Main(String[] args)
		{
			try
			{
#if DEBUG
				WaitForDebugger();
#endif
				var configuration = CustomConfigurationProvider.CollectEnvironmentRelatedConfiguration();
				using(var container = CreateContainer(configuration))
				{
					//Console.WriteLine((String)configuration.GetValue(typeof(String), "ASPNETCORE_ENVIRONMENT"));

					var processor = container.Resolve<IProcessor>();

					processor.Start();

					Console.ReadKey();
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.InnerException?.Message);
				Console.ReadKey();
			}
		}


		// SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
		private static IContainer CreateContainer(IConfigurationRoot configuration)
		{
			var iotHubAssemblies = Collector.LoadAssemblies("MqttMessageProcessor");
			var builder = new ContainerBuilder();

			builder.RegisterServices(iotHubAssemblies);
			//builder.RegisterServiceConfiguration(configuration, iotHubAssemblies);		// TODO: investigate .exe
			builder.RegisterLocalConfiguration(configuration);

			builder.RegisterType<Processor>().AsSelf().AsImplementedInterfaces().SingleInstance();
			builder.RegisterModule(new AutoMapperModule(iotHubAssemblies));

			return builder.Build();
		}
		static void WaitForDebugger()
		{
			Console.WriteLine("Waiting for debugger to attach");
			while(!Debugger.IsAttached)
			{
				Thread.Sleep(100);
			}
			Console.WriteLine("Debugger attached");
		}
	}
}