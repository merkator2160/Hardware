using IotHub.Contracts.Models.Api.DeviceMonitor;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers;

namespace IotHub.Ui.Pages
{
    [Route("devicesUnderTracking")]
    public partial class DevicesUnderTracking
    {
        private DeviceUnderTrackingAm[] _devices;
        private Timer _refreshTimer;


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        [Inject]
        public HttpClient Client { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////

        protected override async Task OnInitializedAsync()
        {
            _devices = await Client.GetFromJsonAsync<DeviceUnderTrackingAm[]>("api/DeviceMonitor/GetAllDeviceInfo");

            _refreshTimer = new Timer(60 * 1000);
            _refreshTimer.Elapsed += Tick;
            _refreshTimer.Start();
        }
        private void Tick(Object sender, ElapsedEventArgs e)
        {
            _devices = Client.GetFromJsonAsync<DeviceUnderTrackingAm[]>("api/DeviceMonitor/GetAllDeviceInfo").Result;

            StateHasChanged();
        }
    }
}