using IotHub.Contracts.Models.Api.DeviceMonitor;
using IotHub.Ui.Clients.IotHubClient.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Timers;
using Timer = System.Timers.Timer;

namespace IotHub.Ui.Pages
{
    [Route("deviceStatus")]
    public partial class DeviceStatus
    {
        private DeviceUnderTrackingAm[] _unavailableDevices;
        private Timer _refreshTimer;


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        [Inject]
        public IIotHubClient Client { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////

        protected override async Task OnInitializedAsync()
        {
            _unavailableDevices = await Client.GetUnavailableDevicesAsync();

            _refreshTimer = new Timer(60 * 1000);
            _refreshTimer.Elapsed += TickAsync;
            _refreshTimer.Start();
        }
        private async void TickAsync(Object sender, ElapsedEventArgs e)
        {
            _unavailableDevices = await Client.GetUnavailableDevicesAsync();

            StateHasChanged();
        }
    }
}