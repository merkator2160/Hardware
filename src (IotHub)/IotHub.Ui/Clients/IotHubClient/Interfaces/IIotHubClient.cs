using IotHub.Contracts.Models.Api.DeviceMonitor;
using IotHub.Contracts.Models.Api.Other;
using System.Threading.Tasks;

namespace IotHub.Ui.Clients.IotHubClient.Interfaces
{
    public interface IIotHubClient
    {
        Task<WeatherForecastAm[]> GetWeatherForecastAsync();
        Task<DeviceUnderTrackingAm[]> GetDevicesUnderTrackingAsync();
        Task<DeviceUnderTrackingAm[]> GetUnavailableDevicesAsync();
    }
}