using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace IotHub.Api
{
	internal class Program
	{
		private static void Main(String[] args)
		{
#if DEBUG
			//WaitForDebugger();
#endif
			CreateHostBuilder(args).Build().Run();
		}
		private static IHostBuilder CreateHostBuilder(String[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.UseServiceProviderFactory(new AutofacServiceProviderFactory())
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
					webBuilder
						.ConfigureLogging((hostingContext, logging) =>
						{
							logging.ClearProviders();
							logging.SetMinimumLevel(LogLevel.Trace);
							logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
							logging.AddDebug();
						});
				});
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
