namespace ApiClients.Http.YandexCloud.Models.Request
{
    public class SynthesizeSpeechRequest
    {
        public String Text { get; set; }
        public String Voice { get; set; }
        public String Format { get; set; }
        public Single Speed { get; set; }
    }
}