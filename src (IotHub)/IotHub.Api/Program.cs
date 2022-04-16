using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.NLog;
using IotHub.Api.Middleware;
using IotHub.Api.Middleware.Cors;
using IotHub.Api.Middleware.Hangfire;
using IotHub.Api.Services.Mqtt;
using IotHub.ApiClients.DependencyInjection;
using IotHub.Common.DependencyInjection;
using IotHub.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using System.Diagnostics;
using System.Reflection;
using uPLibrary.Networking.M2Mqtt.Messages;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace IotHub.Api
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
#if DEBUG
                //WaitForDebugger();
#endif
                RunEnvironmentPrecheck();
                RunApplication(args);
            }
            catch (Exception ex)
            {
                var message = $"{ex.Message}\r\n{ex.StackTrace}";

                logger.Error(ex, message);
                Console.Error.WriteLine(message);

                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
        static void WaitForDebugger()
        {
            Console.WriteLine("Waiting for debugger to attach");
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Debugger attached");
        }
        private static void RunEnvironmentPrecheck()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (String.IsNullOrWhiteSpace(environment))
                throw new EnvironmentVariableNotFoundException($"Unable to start! Environment variable with name \"ASPNETCORE_ENVIRONMENT\" was not found. " +
                                                               $"Possible values: \"{Environments.Development}\", \"{Environments.Production}\"");
        }
        private static void RunApplication(String[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureLogging(builder.Logging, builder.Configuration);
            ConfigureServices(builder.Services, builder.Configuration);
            builder.Host.UseServiceProviderFactory(ConfigureContainer(builder.Configuration));
            builder.Host.UseNLog();

            using (var app = builder.Build())
            {
                ConfigureWebApplication(app, builder.Configuration);
                app.Run();
            }
        }
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped(x => x.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));
            services.AddResponseHandling();
            services.AddCors(CorsMiddleware.AddPolitics);
            services.AddEndpointsApiExplorer();
            services.AddConfiguredSwaggerGen();
            services.AddHangfire();
            services.AddHealthChecks();
            services.AddAuthentication();
            services.AddAuthorization();
            services
                .AddControllers()
                .AddApplicationPart(Assembly.Load(new AssemblyName("IotHub.Api")))
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
        }
        private static AutofacServiceProviderFactory ConfigureContainer(IConfiguration configuration)
        {
            return new AutofacServiceProviderFactory(containerBuilder =>
            {
                var assembliesToScan = Collector.LoadAssemblies("IotHub");

                containerBuilder.RegisterLocalServices();
                containerBuilder.RegisterLocalHangfireJobs();
                containerBuilder.RegisterLocalConfiguration(configuration);

                containerBuilder.RegisterModule<NLogModule>();
                containerBuilder.RegisterModule(new MosquittoClientModule(assembliesToScan));
                containerBuilder.RegisterModule(new AutoMapperModule(assembliesToScan));
                containerBuilder.RegisterModule(new ApiClientModule(configuration));
            });
        }
        private static void ConfigureLogging(ILoggingBuilder logging, IConfiguration configuration)
        {
            // https://stackoverflow.com/questions/8850160/nlog-doent-work-on-iis
            // Edit website permissions in IIS and under security tab give IIS_IUSRS group full privileges.
            // In Application, Pools find the pool your application is using and set some specific user.

            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddConfiguration(configuration.GetSection("Logging"));
            logging.AddDebug();
        }
        private static void ConfigureWebApplication(WebApplication app, IConfiguration configuration)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(CorsPolicies.Development);
            }
            if (app.Environment.IsProduction())
            {
                app.UseHsts();
                app.UseCors(CorsPolicies.Production);
            }

            var scope = app.Services.GetAutofacRoot();
            scope.RegisterJobActivator();

            var hostApplicationLifetime = scope.Resolve<IHostApplicationLifetime>();
            var mosquittoClient = scope.Resolve<MosquittoClient>();

            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                mosquittoClient.Start();
                mosquittoClient.Publish("iotHub/status", "Connected", MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
            });
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                mosquittoClient.Publish("iotHub/status", "Disconnected", MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
                mosquittoClient.Stop();
            });

            app.UseHangfire();
            app.ConfigureHangfireJobs(configuration);
            app.UseConfiguredSwagger();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseGlobalExceptionHandler();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions());
                endpoints.MapControllers();
            });
        }
    }
}
