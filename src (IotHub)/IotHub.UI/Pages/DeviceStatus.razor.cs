﻿using IotHub.Contracts.Models.Api.DeviceMonitor;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers;

namespace IotHub.Ui.Pages
{
    [Route("deviceStatus")]
    public partial class DeviceStatus
    {
        private DeviceUnderTrackingAm[] _unavailableDevices;
        private Timer _refreshTimer;


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        [Inject]
        public HttpClient Client { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////

        protected override async Task OnInitializedAsync()
        {
            _unavailableDevices = await Client.GetFromJsonAsync<DeviceUnderTrackingAm[]>("api/DeviceMonitor/GetUnavailableDeviceInfo");

            _refreshTimer = new Timer(60 * 1000);
            _refreshTimer.Elapsed += Tick;
            _refreshTimer.Start();
        }
        private void Tick(Object sender, ElapsedEventArgs e)
        {
            _unavailableDevices = Client.GetFromJsonAsync<DeviceUnderTrackingAm[]>("api/DeviceMonitor/GetUnavailableDeviceInfo").Result;

            StateHasChanged();
        }
    }
}