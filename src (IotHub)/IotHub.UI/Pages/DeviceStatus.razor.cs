using IotHub.Contracts.Models.Api.DeviceMonitor;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IotHub.UI.Pages
{
    [Route("deviceStatus")]
    public partial class DeviceStatus
    {
        private TrackingDeviceAm[]? _unavailableDevices;


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        [Inject]
        public HttpClient Client { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////

        protected override async Task OnInitializedAsync()
        {
            _unavailableDevices = await Client.GetFromJsonAsync<TrackingDeviceAm[]>("api/DeviceMonitor/GetUnavailableDeviceInfo");
        }
    }
}