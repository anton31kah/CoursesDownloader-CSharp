using System;
using System.Net;
using System.Net.Http;
using CoursesDownloader.Common.ExtensionMethods;

namespace CoursesDownloader.Client.Helpers.HttpClientAutoRedirect
{
    public static partial class HttpClientAutoRedirectExtensions
    {
        private const HttpCompletionOption DefaultCompletionOption = HttpCompletionOption.ResponseContentRead;

        private static bool ShouldRedirect(this HttpStatusCode code)
        {
            var statusCode = (int) code;
            return statusCode >= 300 && statusCode <= 399;
        }

        private static Uri CreateUri(string uri)
        {
            return uri.IsNotNullNorEmpty() ? new Uri(uri, UriKind.RelativeOrAbsolute) : null;
        }
    }
}