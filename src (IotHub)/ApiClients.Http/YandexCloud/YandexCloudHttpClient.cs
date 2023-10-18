using ApiClients.Http.YandexCloud.Models.Config;
using ApiClients.Http.YandexCloud.Models.Exceptions;
using ApiClients.Http.YandexCloud.Models.Request;
using ApiClients.Http.YandexCloud.Models.Response;
using Common.Http;
using System.Globalization;
using System.Net.Http.Headers;

namespace ApiClients.Http.YandexCloud
{
    /// <summary>
    /// https://upread.ru/art.php?id=1085
    /// https://cloud.yandex.com/en/docs/speechkit/tts/request#body_params
    /// https://cloud.yandex.com/en-ru/docs/speechkit/tts/?from=int-console-empty-state
    /// https://cloud.yandex.com/en-ru/docs/speechkit/tts/markup/tts-markup
    /// </summary>
    public class YandexCloudHttpClient : TypedHttpClient
    {
        private readonly YandexCloudHttpClientConfig _config;


        public YandexCloudHttpClient(YandexCloudHttpClientConfig config)
        {
            _config = config;
        }


        // YandexCloudHttpClient //////////////////////////////////////////////////////////////////
        public async Task<GetIamTokenResponseApi> GetIamTokenAsync()
        {
            using (var response = await PostAsync("https://iam.api.cloud.yandex.net/iam/v1/tokens", Serialize(new
            {
                yandexPassportOauthToken = _config.OauthToken
            })))
            {
                if (!response.IsSuccessStatusCode)
                    throw new YandexCloudHttpClientException(HttpMethod.Post, response.StatusCode, response.RequestMessage.RequestUri.AbsoluteUri, await DeserializeAsync<YandexErrorResponseApi>(response));

                return await DeserializeAsync<GetIamTokenResponseApi>(response);
            }
        }
        public async Task<CloudFolders[]> GetFoldersAsync(String iamToken)
        {
            var verb = HttpMethod.Get;
            using (var requestMessage = new HttpRequestMessage(verb, $"https://resource-manager.api.cloud.yandex.net/resource-manager/v1/folders?cloudId={_config.CloudId}"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", iamToken);
                using (var response = await SendAsync(requestMessage))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new YandexCloudHttpClientException(verb, response.StatusCode, response.RequestMessage.RequestUri.AbsoluteUri, await DeserializeAsync<YandexErrorResponseApi>(response));

                    return (await DeserializeAsync<GetFoldersResponse>(response)).Folders;
                }
            }
        }
        public async Task<Byte[]> SynthesizeSpeechAsync(String iamToken, SynthesizeSpeechRequest request)
        {
            var verb = HttpMethod.Post;
            var dict = new Dictionary<String, String>
            {
                { "text", request.Text },
                { "voice", request.Voice },
                {"speed", request.Speed.ToString("g2", CultureInfo.InvariantCulture) },  // 0.1-3
                { "format", request.Format },
                { "folderId", _config.FolderId }
            };

            using (var requestMessage = new HttpRequestMessage(verb, "https://tts.api.cloud.yandex.net/speech/v1/tts:synthesize"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", iamToken);
                requestMessage.Content = new FormUrlEncodedContent(dict);

                using (var response = await SendAsync(requestMessage))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        //var message = await response.Content.ReadAsStringAsync();
                        throw new YandexCloudHttpClientException(verb, response.StatusCode, response.RequestMessage.RequestUri.AbsoluteUri, await DeserializeAsync<YandexErrorResponseApi>(response));
                    }

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }
    }
}