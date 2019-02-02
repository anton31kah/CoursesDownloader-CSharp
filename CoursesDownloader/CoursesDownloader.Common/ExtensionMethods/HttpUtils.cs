using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CoursesDownloader.Common.ExtensionMethods
{
    public static class HttpUtils
    {
        public static Task<string> HtmlContent(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync();
        }

        public static Task<string> GetText(this HttpResponseMessage response)
        {
            return response.HtmlContent();
        }

        public static async Task<string> GetHtmlContentNow(this HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetTextNow(this HttpResponseMessage response)
        {
            return await response.GetHtmlContentNow();
        }

        public static string FindIdFromAncestors(this HtmlNode htmlElement)
        {
            var anchorId = htmlElement.Attributes.FirstOrDefault(e => e.Name == "id")?.Value;

            while (anchorId.IsNullOrEmpty())
            {
                htmlElement = htmlElement.ParentNode;
                anchorId = htmlElement.Attributes.FirstOrDefault(e => e.Name == "id")?.Value;
            }

            return anchorId;
        }
    }
}