using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        public static Task<HttpResponseMessage> SendAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            HttpRequestMessage request)
        {
            return httpClientAutoRedirect.SendAsync(request, DefaultCompletionOption, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> SendAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return httpClientAutoRedirect.SendAsync(request, DefaultCompletionOption, cancellationToken);
        }

        public static Task<HttpResponseMessage> SendAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return httpClientAutoRedirect.SendAsync(request, completionOption, CancellationToken.None);
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpClientAutoRedirect httpClientAutoRedirect,
            HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            var client = httpClientAutoRedirect.HttpClient;
            while (true)
            {
                var response = await client.SendAsync(request, completionOption, cancellationToken);
                if (response.StatusCode.ShouldRedirect())
                {
                    request = new HttpRequestMessage(request.Method, response.Headers.Location);
                    continue;
                }

                return response;
            }
        }
    }
}