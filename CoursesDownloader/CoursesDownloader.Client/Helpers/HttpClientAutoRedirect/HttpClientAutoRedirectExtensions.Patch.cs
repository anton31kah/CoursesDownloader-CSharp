using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        public static Task<HttpResponseMessage> PatchAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, HttpContent content)
        {
            return httpClientAutoRedirect.PatchAsync(CreateUri(requestUri), content);
        }

        public static Task<HttpResponseMessage> PatchAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.PatchAsync(CreateUri(requestUri), content, cancellationToken);
        }

        public static Task<HttpResponseMessage> PatchAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, HttpContent content)
        {
            return httpClientAutoRedirect.PatchAsync(requestUri, content, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> PatchAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri) { Content = content };
            return httpClientAutoRedirect.SendAsync(request, cancellationToken);
        }
    }
}