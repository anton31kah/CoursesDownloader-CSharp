using System.Net.Http;
using System.Threading.Tasks;

namespace CoursesDownloader.Client.Helpers
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> GetHeadersAsync(this HttpClient httpClient, string url)
        {
            return httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}