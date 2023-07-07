using ApiClients.Http.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Contracts.Exceptions.Application;
using Common.DependencyInjection;
using Common.DependencyInjection.Modules;
using IotHub.Api.Middleware;
using IotHub.Api.Middleware.Cors;
using IotHub.Api.Middleware.Hangfire;
using IotHub.Api.Services.Mqtt;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Reflection;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api
{
    internal class Program
    {
        private static void Main(String[] args)
        {
#if DEBUG
                //WaitForDebugger();
#endif
            CheckEnvironment();
            RunApplication(args);
        }
        
        private static void RunApplication(String[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);
            builder.Host.UseServiceProviderFactory(ConfigureContainer(builder.Configuration));
            
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

                containerBuilder.RegisterModule(new MosquittoClientModule(assembliesToScan));
                containerBuilder.RegisterModule(new AutoMapperModule(assembliesToScan));
                containerBuilder.RegisterModule(new HttpClientModule(configuration));
            });
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
        protected static void CheckEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable(CustomConfigurationProvider.DefaultEnvironmentVariableName);
            if (String.IsNullOrWhiteSpace(environment))
                throw new EnvironmentVariableNotFoundException($"Unable to start! Environment variable with name \"{CustomConfigurationProvider.DefaultEnvironmentVariableName}\" was not found. " +
                                                               $"Possible values: \"{Environments.Development}\", \"{Environments.Staging}\", \"{Environments.Production}\"");
        }
        private static void WaitForDebugger()
        {
            Console.WriteLine("Waiting for debugger to attach");
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Debugger attached");
        }
    }
}
