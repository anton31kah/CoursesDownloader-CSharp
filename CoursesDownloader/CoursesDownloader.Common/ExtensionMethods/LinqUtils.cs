using System;
using System.Collections.Generic;
using System.Linq;

namespace CoursesDownloader.Common.ExtensionMethods
{
    public static class LinqUtils
    {
        private static readonly Lazy<Random> LazyRandom = new Lazy<Random>(() => new Random());
        private static Random Random => LazyRandom.Value;

        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }

        public static bool IsEmptyOrNull<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static bool IsEmptyButNotNull<T>(this IEnumerable<T> list)
        {
            return list != null && !list.Any();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> list)
        {
            return list.Any();
        }

        public static bool IsNotEmptyNorNull<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }

        public static IEnumerable<T> SortedUnique<T>(this IEnumerable<T> list)
        {
            return list.Distinct().OrderBy(i => i);
        }

        public static bool IsAscending<T>(this IEnumerable<T> list)
        {
            var enumerable = list.ToList();
            return enumerable.OrderBy(i => i).SequenceEqual(enumerable);
        }

        public static IEnumerable<IEnumerable<int>> ConsecutiveGroupBy(this IEnumerable<int> iterable)
        {
            return iterable.Distinct()
                .OrderBy(i => i)
                .Select((i, idx) => new { i, key = i - idx })
                .GroupBy(tuple => tuple.key, tuple => tuple.i);
        }

        public static IEnumerable<T> TakeRandomN<T>(this IEnumerable<T> list, int n)
        {
            return list.OrderBy(l => Random.Next()).Take(n);
        }

        public static void AddUnique<T>(this IList<T> self, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!self.Contains(item))
                {
                    self.Add(item);
                }
            }
        }
    }
}
