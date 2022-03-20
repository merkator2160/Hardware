using IotHub.Ui;
using IotHub.Ui.Clients.IotHubClient.Interfaces;
using IotHub.UI.Clients.IotHubClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IotHub.UI
{
    internal class Program
    {
        public static async Task Main(String[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.Configuration["ServerConfig:BaseAddress"])
            });
            builder.Services.AddScoped<IIotHubClient>(sp => new IotHubHttpClient
            {
                BaseAddress = new Uri(builder.Configuration["ServerConfig:BaseAddress"])
            });

            await builder.Build().RunAsync();
        }
    }
}