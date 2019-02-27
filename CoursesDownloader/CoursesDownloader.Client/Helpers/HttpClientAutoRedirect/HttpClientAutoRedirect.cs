using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public class HttpClientAutoRedirect : IDisposable
    {
        public HttpClient HttpClient { get; }

        public Uri BaseAddress
        {
            get => HttpClient.BaseAddress;
            set => HttpClient.BaseAddress = value;
        }

        public HttpRequestHeaders DefaultRequestHeaders => HttpClient.DefaultRequestHeaders;

        public long MaxResponseContentBufferSize
        {
            get => HttpClient.MaxResponseContentBufferSize;
            set => HttpClient.MaxResponseContentBufferSize = value;
        }

        public TimeSpan Timeout
        {
            get => HttpClient.Timeout;
            set => HttpClient.Timeout = value;
        }


        public HttpClientAutoRedirect(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClientAutoRedirect(HttpMessageHandler handler)
            : this(new HttpClient(handler))
        {
        }

        public HttpClientAutoRedirect(HttpMessageHandler handler, bool disposeHandler)
            : this(new HttpClient(handler, disposeHandler))
        {
        }

        public void CancelPendingRequests()
        {
            HttpClient.CancelPendingRequests();
        }

        public override bool Equals(object obj)
        {
            return HttpClient.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HttpClient.GetHashCode();
        }

        public override string ToString()
        {
            return HttpClient.ToString();
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}