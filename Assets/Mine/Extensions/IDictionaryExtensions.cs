using System.Collections.Generic;

namespace Mine.Extensions
{
    public static class IDictionaryExtensions
    {
        public static void Update<TKey, TVal>(this IDictionary<TKey, TVal> orig, IDictionary<TKey, TVal> other)
        {
            foreach (var (key, value) in other)
            {
                orig[key] = value;
            }
        }
    }
}