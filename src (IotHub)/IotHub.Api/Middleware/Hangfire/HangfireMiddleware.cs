using Autofac;
using Hangfire;
using Hangfire.Auth;
using Hangfire.MemoryStorage;
using IotHub.Api.Middleware.Hangfire.Jobs;
using IotHub.Api.Services.Models.Config;
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
        }
        public static void AddHangfire(this IServiceCollection services)
        {
            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "default" };
            });
            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });
        }
        public static void RegisterJobActivator(this ILifetimeScope scope)
        {
            GlobalConfiguration.Configuration.UseAutofacActivator(scope);
        }
        public static void ConfigureHangfireJobs(this IApplicationBuilder app, IConfiguration configuration)
        {
#if DEVELOPMENT
            //BackgroundJob.Enqueue<UpTimeJob>(p => p.Execute());
            //BackgroundJob.Enqueue<GreenhouseLightTurnOnJob>(p => p.Execute());
            //BackgroundJob.Enqueue<GreenhouseLightTurnOffJob>(p => p.Execute());

            ConfigureRecurringJobs(configuration);
#else
            ConfigureOneTimeJobs();
            ConfigureRecurringJobs(configuration);
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
            //BackgroundJob.Enqueue<GreenhouseLightCelestialSchedulerJob>(p => p.Execute());
        }
        private static void ConfigureRecurringJobs(IConfiguration configuration)
        {
            RecurringJob.AddOrUpdate<UpTimeJob>(
                p => p.Execute(),
                "0 * * ? * *",
                timeZone: TimeZoneInfo.Local);

            //RecurringJob.AddOrUpdate<GreenhouseLightCelestialSchedulerJob>(
            //     p => p.Execute(),
            //     "0 0 0 ? * *",
            //     timeZone: TimeZoneInfo.Local);

            var dayDurationConfig = configuration.GetSection("DayDurationConfig").Get<DayDurationConfig>();

            RecurringJob.AddOrUpdate<GreenhouseLightTurnOnJob>(
                p => p.Execute(),
                $"0 0 {dayDurationConfig.DayBeginHour} ? * *",
                timeZone: TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<GreenhouseLightTurnOffJob>(
                p => p.Execute(),
                $"0 0 {dayDurationConfig.DayEndHour} ? * *",
                timeZone: TimeZoneInfo.Local);
        }
    }
}