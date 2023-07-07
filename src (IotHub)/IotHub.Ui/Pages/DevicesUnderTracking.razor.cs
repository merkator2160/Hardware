using Common.Contracts.Api.DeviceMonitor;
using IotHub.Ui.Clients.IotHubClient.Interfaces;
using Microsoft.AspNetCore.Components;

namespace IotHub.Ui.Pages
{
    [Route("devicesUnderTracking")]
    public partial class DevicesUnderTracking
    {
        private DeviceUnderTrackingAm[] _devices;


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        [Inject]
        public IIotHubClient Client { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////

        protected override async Task OnInitializedAsync()
        {
            _devices = await Client.GetDevicesUnderTrackingAsync();
        }
    }
}