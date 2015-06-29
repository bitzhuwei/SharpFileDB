using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace SharpFileDB
{
    internal static class DictionaryExtensions
    {
        public static ushort NextIndex<T>(this Dictionary<UInt64, T> dict)
        {
            ushort next = 0;

            while (dict.ContainsKey(next))
            {
                next++;
            }

            return next;
        }

        public static V GetOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V result;

            if (dict.TryGetValue(key, out result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
