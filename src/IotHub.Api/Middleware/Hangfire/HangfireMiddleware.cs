using Autofac;
using Hangfire;
using Hangfire.MemoryStorage;
using IotHub.Common.Hangfire.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace IotHub.Api.Middleware.Hangfire
{
	/// <summary>
	/// https://stackoverflow.com/questions/44383174/hangfire-with-horizontal-scaling
	/// https://stackoverflow.com/questions/42201809/hangfire-recurring-job-on-every-server/42202844
	/// http://docs.hangfire.io/en/latest/background-processing/configuring-queues.html
	/// </summary>
	internal static class HangfireMiddleware
	{
		public static void UseHangfire(this IApplicationBuilder app)
		{
			app.UseHangfireDashboard("/hangfire", new DashboardOptions()
			{
				Authorization = new[]
				{
					new FreeAuthorizationFilter()
				}
			});
			app.UseHangfireServer(new BackgroundJobServerOptions()
			{
				Queues = new[] { "default", CreateEnvironmentDependentQueueName() }
			});
		}
		public static void AddHangfire(this IServiceCollection services)
		{
			services.AddHangfire(config =>
			{
				config.UseMemoryStorage();
			});
		}
		public static void RegisterJobActivator(this ILifetimeScope scope)
		{
			GlobalConfiguration.Configuration.UseAutofacActivator(scope);
		}
		public static void ConfigureHangfireJobs(this IApplicationBuilder app)
		{
#if DEVELOPMENT
			var parameter = new SampleJobParameter()
			{
				Parameter = $"{nameof(SampleParametrizedAsyncJob)} is executing"
			};

			BackgroundJob.Enqueue<SampleParametrizedAsyncJob>(p => p.ExecuteAsync(parameter));
			RecurringJob.AddOrUpdate<SampleParametrizedAsyncJob>(
				p => p.ExecuteAsync(parameter),
				Cron.Minutely,
				timeZone: TimeZoneInfo.Utc,
				queue: CreateEnvironmentDependentQueueName());
#else
			ConfigureOneTimeJobs();
			ConfigureRecurringJobs();
#endif
		}


		// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
		private static String CreateEnvironmentDependentQueueName()
		{
			return Environment.MachineName.Replace("-", "").ToLower(CultureInfo.InvariantCulture);
		}
		private static void ConfigureOneTimeJobs()
		{
			//BackgroundJob.Enqueue<DeliveryReportJob>(p => p.ExecuteAsync());
		}
		private static void ConfigureRecurringJobs()
		{
			//RecurringJob.AddOrUpdate<SampleJob>(
			// p => p.ExecuteAsync(),
			// Cron.Minutely,
			// timeZone: TimeZoneInfo.Utc,
			// queue: CreateEnvironmentDependentQueueName());
		}
	}
}