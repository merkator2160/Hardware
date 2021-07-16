using Autofac;
using Autofac.Extras.NLog;
using IotHub.Api.Middleware;
using IotHub.Api.Middleware.Cors;
using IotHub.Api.Middleware.Hangfire;
using IotHub.Api.Services;
using IotHub.ApiClients.DependencyInjection;
using IotHub.Common.Config;
using IotHub.Common.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api
{
	internal class Startup
	{
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _env;


		public Startup(IWebHostEnvironment env)
		{
			_env = env;
			_configuration = CustomConfigurationProvider.CreateConfiguration(env.EnvironmentName, env.ContentRootPath);
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
				.AddScoped(x => x.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));
			services.AddCors(CorsMiddleware.AddPolitics);
			services.AddConfiguredSwaggerGen();
			services.AddHangfire();
			services.AddHealthChecks();
			services.ConfigureResponseHandling();
			services
				.AddControllers()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = new DefaultContractResolver();
				})
				.SetCompatibilityVersion(CompatibilityVersion.Latest);
		}
		public void ConfigureContainer(ContainerBuilder builder)
		{
			var assembliesToScan = Collector.LoadAssemblies("DenverTraffic");

			builder.RegisterLocalServices();
			builder.RegisterLocalHangfireJobs();
			builder.RegisterLocalConfiguration(_configuration);

			builder.RegisterType<MosquittoClient>().AsSelf().AsImplementedInterfaces().SingleInstance();

			builder.RegisterModule<NLogModule>();
			builder.RegisterModule(new AutoMapperModule(assembliesToScan));
			builder.RegisterModule(new ApiClientModule(_configuration));
		}
		public void Configure(IApplicationBuilder app, MosquittoClient mosquittoClient, IHostApplicationLifetime hostApplicationLifetime)
		{
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


			if(_env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseCors(CorsPolicies.Development);
			}
			if(_env.IsStaging())
			{
				app.UseDeveloperExceptionPage();
				app.UseCors(CorsPolicies.Staging);
			}
			if(_env.IsProduction())
			{
				app.UseHsts();
				app.UseCors(CorsPolicies.Production);
			}

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseHangfire();
			app.ConfigureHangfireJobs();
			app.UseConfiguredSwagger();
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
