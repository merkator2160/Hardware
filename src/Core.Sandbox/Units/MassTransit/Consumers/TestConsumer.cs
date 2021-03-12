using Core.Sandbox.Units.MassTransit.Models;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Core.Sandbox.Units.MassTransit.Consumers
{
	public class TestConsumer : IConsumer<BasicCommand>
	{
		// IConsumer<TestMessage> /////////////////////////////////////////////////////////////////
		public async Task Consume(ConsumeContext<BasicCommand> context)
		{
			await Console.Out.WriteLineAsync($"Received {context.Message.Value}");
		}
	}
}