using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        public static Task<HttpResponseMessage> GetHeadersAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri)
        {
            return httpClientAutoRedirect.GetHeadersAsync(CreateUri(requestUri));
        }

        public static Task<HttpResponseMessage> GetHeadersAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri)
        {
            return httpClientAutoRedirect.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
