using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers
{
    public static class HttpClientExtensions
    {
        private static bool ShouldRedirect(this HttpStatusCode code)
        {
            var codesWhenToRedirect = new[]
            {
                HttpStatusCode.Ambiguous,
                HttpStatusCode.Found,
                HttpStatusCode.Moved,
                HttpStatusCode.MovedPermanently,
                HttpStatusCode.MultipleChoices,
                HttpStatusCode.Redirect,
                HttpStatusCode.RedirectKeepVerb,
                HttpStatusCode.RedirectMethod,
                HttpStatusCode.SeeOther,
                HttpStatusCode.TemporaryRedirect,
            };

            return codesWhenToRedirect.Contains(code);
        }

        public static async Task<HttpResponseMessage> GetAsyncHttp(this HttpClient httpClient, Uri uri)
        {
            var response = await httpClient.GetAsync(uri);
            if (response.StatusCode.ShouldRedirect())
            {
                return await httpClient.GetAsyncHttp(response.Headers.Location);
            }

            return response;
        }

        public static async Task<HttpResponseMessage> GetAsyncHttp(this HttpClient httpClient, Uri uri, HttpCompletionOption option)
        {
            var response = await httpClient.GetAsync(uri, option);
            if (response.StatusCode.ShouldRedirect())
            {
                return await httpClient.GetAsyncHttp(response.Headers.Location, option);
            }

            return response;
        }

        public static Task<HttpResponseMessage> GetAsyncHttp(this HttpClient httpClient, string url)
        {
            return httpClient.GetAsyncHttp(new Uri(url));
        }

        public static Task<HttpResponseMessage> GetAsyncHttp(this HttpClient httpClient, string url, HttpCompletionOption option)
        {
            return httpClient.GetAsyncHttp(new Uri(url), option);
        }

        public static Task<HttpResponseMessage> GetHeadersAsyncHttp(this HttpClient httpClient, string url)
        {
            return httpClient.GetAsyncHttp(url, HttpCompletionOption.ResponseHeadersRead);
        }

        public static async Task<HttpResponseMessage> PostAsyncHttp(this HttpClient httpClient, Uri uri, HttpContent content)
        {
            var response = await httpClient.PostAsync(uri, content);
            if (response.StatusCode.ShouldRedirect())
            {
                return await httpClient.PostAsyncHttp(response.Headers.Location, content);
            }

            return response;
        }

        public static Task<HttpResponseMessage> PostAsyncHttp(this HttpClient httpClient, string url, HttpContent content)
        {
            return httpClient.PostAsyncHttp(new Uri(url), content);
        }

        public static async Task<string> GetStringAsyncHttp(this HttpClient httpClient, Uri uri)
        {
            var response = await httpClient.GetAsyncHttp(uri);
            return await response.Content.ReadAsStringAsync();
        }

        public static Task<string> GetStringAsyncHttp(this HttpClient httpClient, string url)
        {
            return httpClient.GetStringAsyncHttp(new Uri(url));
        }

        public static async Task<Stream> GetStreamAsyncHttp(this HttpClient httpClient, Uri uri)
        {
            var response = await httpClient.GetAsyncHttp(uri);
            return await response.Content.ReadAsStreamAsync();
        }

        public static Task<Stream> GetStreamAsyncHttp(this HttpClient httpClient, string url)
        {
            return httpClient.GetStreamAsyncHttp(new Uri(url));
        }
    }
}