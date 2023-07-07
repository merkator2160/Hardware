using Common.Contracts.Api.Other;
using IotHub.Ui.Clients.IotHubClient.Interfaces;
using Microsoft.AspNetCore.Components;

namespace IotHub.Ui.Pages
{
    [Route("fetchdata")]
    public partial class FetchData
    {
        private WeatherForecastAm[] _forecasts;


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        [Inject]
        public IIotHubClient Client { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        protected override async Task OnInitializedAsync()
        {
            _forecasts = await Client.GetWeatherForecastAsync();
        }
    }
}