using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri)
        {
            return httpClientAutoRedirect.GetAsync(CreateUri(requestUri));
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.GetAsync(CreateUri(requestUri), cancellationToken);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, HttpCompletionOption completionOption)
        {
            return httpClientAutoRedirect.GetAsync(CreateUri(requestUri), completionOption);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.GetAsync(CreateUri(requestUri), completionOption, cancellationToken);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri)
        {
            return httpClientAutoRedirect.GetAsync(requestUri, DefaultCompletionOption);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.GetAsync(requestUri, DefaultCompletionOption, cancellationToken);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, HttpCompletionOption completionOption)
        {
            return httpClientAutoRedirect.GetAsync(requestUri, completionOption, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
        }
    }
}