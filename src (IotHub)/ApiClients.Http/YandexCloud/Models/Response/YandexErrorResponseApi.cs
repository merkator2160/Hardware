using Newtonsoft.Json;

namespace ApiClients.Http.YandexCloud.Models.Response
{
    public class YandexErrorResponseApi
    {
        [JsonProperty("error_code")]
        public String Code { get; set; }

        [JsonProperty("error_message")]
        public String Message { get; set; }

        public Dictionary<String, String> Details { get; set; }
    }
}