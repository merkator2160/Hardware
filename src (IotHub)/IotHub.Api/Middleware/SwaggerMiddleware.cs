using Microsoft.OpenApi.Models;
using System.Reflection;

namespace IotHub.Api.Middleware
{
    internal static class SwaggerMiddleware
    {
        private const String _documentName = "IotHubApi";


        public static void AddConfiguredSwaggerGen(this IServiceCollection services)
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_documentName, new OpenApiInfo
                {
                    Version = $"v{assemblyVersion}",
                    Title = "IoT hub service",
                    Description = "IoT hub API",
                    Contact = new OpenApiContact()
                    {
                        Name = "Aleksandrov Evgeniy",
                        Email = "ulthane2160@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/evgeniy-alexandrov-967388100")
                    }
                });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
            });
        }
        public static void UseConfiguredSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}.json";
                options.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{_documentName}.json", "IoT hub API");
            });
        }
    }
}
