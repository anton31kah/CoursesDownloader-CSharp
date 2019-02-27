using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        public static Task<HttpResponseMessage> PostAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, HttpContent content)
        {
            return httpClientAutoRedirect.PostAsync(CreateUri(requestUri), content);
        }

        public static Task<HttpResponseMessage> PostAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.PostAsync(CreateUri(requestUri), content, cancellationToken);
        }

        public static Task<HttpResponseMessage> PostAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, HttpContent content)
        {
            return httpClientAutoRedirect.PostAsync(requestUri, content, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> PostAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = content};
            return httpClientAutoRedirect.SendAsync(request, cancellationToken);
        }
    }
}