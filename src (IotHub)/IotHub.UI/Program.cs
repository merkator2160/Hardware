using IotHub.Ui.Clients.IotHubClient;
using IotHub.Ui.Clients.IotHubClient.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace IotHub.Ui
{
    internal class Program
    {
        public static async Task Main(String[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<IIotHubClient>(sp => new IotHubHttpClient
            {
                BaseAddress = new Uri(builder.Configuration["ServerConfig:BaseAddress"])
            });

            await builder.Build().RunAsync();
        }
    }
}