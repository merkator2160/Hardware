using Common.Contracts.Api.DeviceMonitor;
using Common.Contracts.Api.Other;

namespace IotHub.Ui.Clients.IotHubClient.Interfaces
{
    public interface IIotHubClient
    {
        Task<WeatherForecastAm[]> GetWeatherForecastAsync();
        Task<DeviceUnderTrackingAm[]> GetDevicesUnderTrackingAsync();
        Task<DeviceUnderTrackingAm[]> GetUnavailableDevicesAsync();
    }
}