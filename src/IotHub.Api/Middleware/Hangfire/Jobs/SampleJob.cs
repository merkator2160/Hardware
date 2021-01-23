using Hangfire;
using IotHub.Common.Hangfire.Interfaces;
using NLog;
using System;
using System.Threading.Tasks;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
	internal class SampleAsyncJob : IAsyncJob
	{
		private readonly ILogger _logger;


		public SampleAsyncJob(ILogger logger)
		{
			_logger = logger;
		}


		// IJob ///////////////////////////////////////////////////////////////////////////////////
		[AutomaticRetry(Attempts = 0)]
		public async Task ExecuteAsync()
		{
			try
			{
				Console.WriteLine($"{nameof(SampleParametrizedAsyncJob)} is executing");

				await Task.Delay(1000);
			}
			catch(Exception ex)
			{
				throw;
			}
		}
	}
}