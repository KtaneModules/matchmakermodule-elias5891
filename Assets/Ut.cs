﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Rnd = UnityEngine.Random;

namespace HumanResources
{
    static class Ut
    {
        public static T Shuffle<T>(this T list) where T : IList
        {
            if (list == null)
                throw new ArgumentNullException("list");
            for (int j = list.Count; j >= 1; j--)
            {
                int item = Rnd.Range(0, j);
                if (item < j - 1)
                {
                    var t = list[item];
                    list[item] = list[j - 1];
                    list[j - 1] = t;
                }
            }
            return list;
        }

        public static string JoinString<T>(this IEnumerable<T> values, string separator = null, string prefix = null, string suffix = null, string lastSeparator = null)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (lastSeparator == null)
                lastSeparator = separator;

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return "";

                // Optimise the case where there is only one element
                var one = enumerator.Current;
                if (!enumerator.MoveNext())
                    return prefix + one + suffix;

                // Optimise the case where there are only two elements
                var two = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    // Optimise the (common) case where there is no prefix/suffix; this prevents an array allocation when calling string.Concat()
                    if (prefix == null && suffix == null)
                        return one + lastSeparator + two;
                    return prefix + one + suffix + lastSeparator + prefix + two + suffix;
                }

                StringBuilder sb = new StringBuilder()
                    .Append(prefix).Append(one).Append(suffix).Append(separator)
                    .Append(prefix).Append(two).Append(suffix);
                var prev = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    sb.Append(separator).Append(prefix).Append(prev).Append(suffix);
                    prev = enumerator.Current;
                }
                sb.Append(lastSeparator).Append(prefix).Append(prev).Append(suffix);
                return sb.ToString();
            }
        }
    }
}