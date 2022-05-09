using IOTCS.EdgeGateway.Core.Collections;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace System
{
    public static class StringExtensions
    {
        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull(str, nameof(str));

            if (str.EndsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }

        public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull(str, nameof(str));

            if (str.StartsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return c + str;
        }

        public static Boolean EndsWithIgnoreCase(this String value, params String[] strs)
        {
            if (value == null || String.IsNullOrEmpty(value)) return false;

            foreach (var item in strs)
            {
                if (value.EndsWith(item, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public static String F(this String value, params Object[] args)
        {
            if (String.IsNullOrEmpty(value)) return value;

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] is DateTime dt)
                {
                    // 没有写格式化字符串的时间参数，一律转为标准时间字符串
                    if (value.Contains("{" + i + "}")) args[i] = dt.ToFullString();
                }
            }

            return String.Format(value, args);
        }

        public static StringBuilder Separate(this StringBuilder sb, String separator)
        {
            if (/*sb == null ||*/ String.IsNullOrEmpty(separator)) return sb;

            if (sb.Length > 0) sb.Append(separator);

            return sb;
        }

        public static String Join<T>(this IEnumerable<T> value, String separator = ",", Func<T, Object> func = null)
        {
            var sb = Pool.StringBuilder.Get();
            if (value != null)
            {
                if (func == null) func = obj => "{0}".F(obj);
                foreach (var item in value)
                {
                    sb.Separate(separator).Append(func(item));
                }
            }
            return sb.Put(true);
        }
    }
}
