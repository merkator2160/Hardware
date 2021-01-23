using Hangfire;
using IotHub.Api.Middleware.Hangfire.Models;
using IotHub.Common.Hangfire.Interfaces;
using NLog;
using System;
using System.Threading.Tasks;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
	internal class SampleParametrizedAsyncJob : IAsyncJob<SampleJobParameter>
	{
		private readonly ILogger _logger;


		public SampleParametrizedAsyncJob(ILogger logger)
		{
			_logger = logger;
		}


		// IJob ///////////////////////////////////////////////////////////////////////////////////
		[AutomaticRetry(Attempts = 0)]
		public async Task ExecuteAsync(SampleJobParameter parameter)
		{
			try
			{
				Console.WriteLine(parameter.Parameter);

				await Task.Delay(1000);
			}
			catch(Exception ex)
			{
				throw;
			}
		}
	}
}