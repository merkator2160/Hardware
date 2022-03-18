using IotHub.Contracts.Models.Api.Other;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IotHub.Ui.Pages
{
    [Route("fetchdata")]
    public partial class FetchData
    {
        private WeatherForecastAm[] _forecasts;


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        [Inject]
        public HttpClient Client { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        protected override async Task OnInitializedAsync()
        {
            _forecasts = await Client.GetFromJsonAsync<WeatherForecastAm[]>("api/Debug/GetWeatherForecast");
        }
    }
}