using Common.Contracts.Api.DeviceMonitor;
using Common.Contracts.Api.Other;
using IotHub.Ui.Clients.IotHubClient.Interfaces;
using System.Net.Http.Json;

namespace IotHub.Ui.Clients.IotHubClient
{
    public class IotHubHttpClient : HttpClient, IIotHubClient
    {
        // IIotHubClient //////////////////////////////////////////////////////////////////////////
        public async Task<WeatherForecastAm[]> GetWeatherForecastAsync()
        {
            return await this.GetFromJsonAsync<WeatherForecastAm[]>("api/Debug/GetWeatherForecast");
        }
        public async Task<DeviceUnderTrackingAm[]> GetDevicesUnderTrackingAsync()
        {
            return await this.GetFromJsonAsync<DeviceUnderTrackingAm[]>("api/DeviceMonitor/GetAllDeviceInfo");
        }
        public async Task<DeviceUnderTrackingAm[]> GetUnavailableDevicesAsync()
        {
            return await this.GetFromJsonAsync<DeviceUnderTrackingAm[]>("api/DeviceMonitor/GetUnavailableDeviceInfo");
        }
    }
}