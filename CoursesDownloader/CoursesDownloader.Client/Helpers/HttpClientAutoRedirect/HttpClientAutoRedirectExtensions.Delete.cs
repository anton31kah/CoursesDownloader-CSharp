using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        public static Task<HttpResponseMessage> DeleteAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri)
        {
            return httpClientAutoRedirect.DeleteAsync(CreateUri(requestUri));
        }

        public static Task<HttpResponseMessage> DeleteAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.DeleteAsync(CreateUri(requestUri), cancellationToken);
        }
        
        public static Task<HttpResponseMessage> DeleteAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri)
        {
            return httpClientAutoRedirect.DeleteAsync(requestUri, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> DeleteAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
        }
    }
}