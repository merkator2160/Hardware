namespace ApiClients.Http.YandexCloud.Models.Config
{
    public class YandexCloudHttpClientConfig
    {
        public String OauthToken { get; set; }
        public String CloudId { get; set; }
        public String FolderId { get; set; }
        public Int32 IamTokenRefreshThresholdSec { get; set; }
    }
}