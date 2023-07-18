using ApiClients.Http.YandexCloud.Models.Response;
using Common.Contracts.Exceptions.Application;
using System.Net;

namespace ApiClients.Http.YandexCloud.Models.Exceptions
{
    public class YandexCloudHttpClientException : HttpServerException
    {
        public YandexCloudHttpClientException(HttpMethod verb, HttpStatusCode statusCode, String uri, YandexErrorResponseApi error) : base(verb, statusCode, uri, error.Message)
        {
            YandexError = error;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public YandexErrorResponseApi YandexError { get; }
    }
}