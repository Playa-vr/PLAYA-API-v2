using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlayaApiV2.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> collection) => collection ?? Array.Empty<T>();

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection switch
        {
            null => true,
            IReadOnlyCollection<T> c => c.Count == 0,
            ICollection<T> c => c.Count == 0,
            ICollection c => c.Count == 0,
            string s => s.Length == 0,
            _ => !collection.Any(),
        };
    }
}
