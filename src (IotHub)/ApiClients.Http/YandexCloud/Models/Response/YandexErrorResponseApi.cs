namespace ApiClients.Http.YandexCloud.Models.Response
{
    public class YandexErrorResponseApi
    {
        public String Code { get; set; }
        public String Message { get; set; }
        public Dictionary<String, String> Details { get; set; }
    }
}