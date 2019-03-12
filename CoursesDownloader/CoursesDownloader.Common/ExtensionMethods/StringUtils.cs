using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CoursesDownloader.Common.ExtensionMethods
{
    public static class StringUtils
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotEmpty(this string str)
        {
            return str.Length != 0;
        }

        public static bool IsNotNullNorEmpty(this string str)
        {
            return !str.IsNullOrEmpty();
        }
        
        public static string IfIsNullOrEmpty(this string str, string replacement)
        {
            return str.IsNullOrEmpty() ? replacement : str;
        }
        
        public static string IfIsNotNullNorEmpty(this string str, Func<string> replacement, string otherwise = "")
        {
            return str.IsNotNullNorEmpty() ? replacement.Invoke() : otherwise;
        }
        
        public static string Join<T>(this IEnumerable<T> list, string joiner = "\n")
        {
            return string.Join(joiner, list);
        }

        public static string Replace(this string str, char[] toReplace, string replacement)
        {
            return str.Split(toReplace, StringSplitOptions.RemoveEmptyEntries).Join(replacement);
            
            // replaces each char, instead of combining multiple underscores
            // return str.Split(toReplace).Join(replacement);
        }
        
        public static string TrimInnerSpaces(this string text)
        {
            return Regex.Replace(text, @"\s+", " ");
        }

        public static string DecodeUtf8(this string text)
        {
            return Encoding.UTF8.GetString(Array.ConvertAll(Regex.Unescape(text).ToCharArray(), c => (byte) c));
        }

        public static string DecodeHtml(this string text)
        {
            return WebUtility.HtmlDecode(text);
        }

        public static string DecodeUrl(this string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
            {
                url = newUrl;
            }

            return newUrl;
        }

        public static string EscapeQuotes(this string text)
        {
            return text.Replace("\"", "");
        }

        public static string FirstLine(this string text)
        {
            return text.TakeWhile(c => c != '\n').Join("");
        }

        public static string WithoutFirstLine(this string text)
        {
            return text.SkipWhile(c => c != '\n').Skip(1).Join("");
        }
    }
}
