using Autofac;
using Hangfire;
using Hangfire.MemoryStorage;
using IotHub.Api.Middleware.Hangfire.Jobs;
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
				Queues = new[] { "default" }
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
			BackgroundJob.Enqueue<UpTimeJob>(p => p.Execute());
			BackgroundJob.Enqueue<SideRoomKaktusLightJob>(p => p.Execute());
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
			BackgroundJob.Enqueue<UpTimeJob>(p => p.Execute());
			BackgroundJob.Enqueue<SideRoomKaktusLightJob>(p => p.Execute());
		}
		private static void ConfigureRecurringJobs()
		{
			RecurringJob.AddOrUpdate<UpTimeJob>(
				p => p.Execute(),
				"0 * * ? * *",
				timeZone: TimeZoneInfo.Local);

			RecurringJob.AddOrUpdate<SideRoomKaktusLightJob>(
				p => p.Execute(),
				"0 */10 * ? * *",
				timeZone: TimeZoneInfo.Local);
		}
	}
}