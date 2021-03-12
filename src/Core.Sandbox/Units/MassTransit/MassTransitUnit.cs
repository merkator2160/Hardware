using Autofac;
using Core.Sandbox.Units.MassTransit.Models;
using MassTransit;
using System;
using System.Reflection;
using System.Threading;

namespace Core.Sandbox.Units.MassTransit
{
	internal static class MassTransitUnit
	{
		public static void Run()
		{
			using(var container = CreateContainer())
			{
				var busControl = container.Resolve<IBusControl>();
				busControl.Start();

				Thread.Sleep(5000);

				var publishEndpoint = container.Resolve<IPublishEndpoint>();
				for(var i = 0; i < 10; i++)
				{
					publishEndpoint.Publish(new BasicCommand()
					{
						Value = i
					}).GetAwaiter().GetResult();

					Console.WriteLine($"Sent {i}");
					Thread.Sleep(1000);
				}

				Console.ReadKey();

				busControl.Stop();
			}
		}


		// SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
		private static IContainer CreateContainer()
		{
			var currentAssembly = Assembly.GetExecutingAssembly();
			var builder = new ContainerBuilder();

			builder.AddMassTransit(x =>
			{
				x.AddConsumers(currentAssembly);
				x.UsingInMemory((context, cfg) =>
				{
					cfg.TransportConcurrencyLimit = 100;

					cfg.ConfigureEndpoints(context);
				});
			});

			return builder.Build();
		}
	}
}