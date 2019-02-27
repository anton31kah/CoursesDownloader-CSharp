using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        public static Task<Stream> GetStreamAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri)
        {
            return httpClientAutoRedirect.GetStreamAsync(CreateUri(requestUri));
        }

        public static Task<Stream> GetStreamAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri)
        {
            var task = httpClientAutoRedirect.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
            return GetStreamAsyncCore(task);
        }

        private static async Task<Stream> GetStreamAsyncCore(Task<HttpResponseMessage> getTask)
        {
            var response = await getTask.ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var c = response.Content;
            return c != null ?
                await c.ReadAsStreamAsync().ConfigureAwait(false) :
                Stream.Null;
        }

        public static Task<string> GetStringAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            string requestUri)
        {
            return httpClientAutoRedirect.GetStringAsync(CreateUri(requestUri));
        }

        public static Task<string> GetStringAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            Uri requestUri)
        {
            var task = httpClientAutoRedirect.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
            return FinishGetStringAsync(task);
        }

        private static async Task<string> FinishGetStringAsync(Task<HttpResponseMessage> getTask)
        {
            var response = await getTask.ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var c = response.Content;
            return c != null ?
                await c.ReadAsStringAsync().ConfigureAwait(false) :
                string.Empty;
        }

    }
}