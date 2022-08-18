﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Reflection;

namespace System
{
    public class DefaultConvert
    {
        private static DateTime _dt1970 = new DateTime(1970, 1, 1);
        private static DateTimeOffset _dto1970 = new DateTimeOffset(new DateTime(1970, 1, 1));

        /// <summary>转为整数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）、时间（Unix秒）</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public virtual Int32 ToInt(Object value, Int32 defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is String str)
            {
                // 拷贝而来的逗号分隔整数
                str = str.Replace(",", null);
                str = ToDBC(str).Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (Int32.TryParse(str, out var n)) return n;
                return defaultValue;
            }

            // 特殊处理时间，转Unix秒
            if (value is DateTime dt)
            {
                if (dt == DateTime.MinValue) return 0;

                //// 先转UTC时间再相减，以得到绝对时间差
                //return (Int32)(dt.ToUniversalTime() - _dt1970).TotalSeconds;
                return (Int32)(dt - _dt1970).TotalSeconds;
            }
            if (value is DateTimeOffset dto)
            {
                if (dto == DateTimeOffset.MinValue) return 0;

                return (Int32)(dto - _dto1970).TotalSeconds;
            }

            if (value is Byte[] buf)
            {
                if (buf == null || buf.Length < 1) return defaultValue;

                switch (buf.Length)
                {
                    case 1:
                        return buf[0];
                    case 2:
                        return BitConverter.ToInt16(buf, 0);
                    case 3:
                        return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                    case 4:
                        return BitConverter.ToInt32(buf, 0);
                    default:
                        break;
                }
            }

            try
            {
                return Convert.ToInt32(value);
            }
            catch { return defaultValue; }
        }

        /// <summary>转为长整数。支持字符串、全角、字节数组（小端）、时间（Unix毫秒）</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public virtual Int64 ToLong(Object value, Int64 defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is String str)
            {
                // 拷贝而来的逗号分隔整数
                str = str.Replace(",", null);
                str = ToDBC(str).Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (Int64.TryParse(str, out var n)) return n;
                return defaultValue;
            }

            // 特殊处理时间，转Unix毫秒
            if (value is DateTime dt)
            {
                if (dt == DateTime.MinValue) return 0;

                //// 先转UTC时间再相减，以得到绝对时间差
                //return (Int32)(dt.ToUniversalTime() - _dt1970).TotalSeconds;
                return (Int64)(dt - _dt1970).TotalMilliseconds;
            }
            if (value is DateTimeOffset dto)
            {
                if (dto == DateTimeOffset.MinValue) return 0;

                return (Int64)(dto - _dto1970).TotalMilliseconds;
            }

            if (value is Byte[] buf)
            {
                if (buf == null || buf.Length < 1) return defaultValue;

                switch (buf.Length)
                {
                    case 1:
                        return buf[0];
                    case 2:
                        return BitConverter.ToInt16(buf, 0);
                    case 3:
                        return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                    case 4:
                        return BitConverter.ToInt32(buf, 0);
                    case 8:
                        return BitConverter.ToInt64(buf, 0);
                    default:
                        break;
                }
            }

            //暂时不做处理  先处理异常转换
            try
            {
                return Convert.ToInt64(value);
            }
            catch { return defaultValue; }
        }

        /// <summary>转为浮点数</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public virtual Double ToDouble(Object value, Double defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is String str)
            {
                str = ToDBC(str).Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (Double.TryParse(str, out var n)) return n;
                return defaultValue;
            }
            else if (value is Byte[] buf)
            {
                if (buf == null || buf.Length < 1) return defaultValue;

                switch (buf.Length)
                {
                    case 1:
                        return buf[0];
                    case 2:
                        return BitConverter.ToInt16(buf, 0);
                    case 3:
                        return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                    case 4:
                        return BitConverter.ToInt32(buf, 0);
                    default:
                        // 凑够8字节
                        if (buf.Length < 8)
                        {
                            var bts = new Byte[8];
                            Buffer.BlockCopy(buf, 0, bts, 0, buf.Length);
                            buf = bts;
                        }
                        return BitConverter.ToDouble(buf, 0);
                }
            }

            try
            {
                return Convert.ToDouble(value);
            }
            catch { return defaultValue; }
        }

        /// <summary>转为布尔型。支持大小写True/False、0和非零</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public virtual Boolean ToBoolean(Object value, Boolean defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的
            if (value is String str)
            {
                //str = ToDBC(str).Trim();
                str = str.Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (Boolean.TryParse(str, out var b)) return b;

                if (String.Equals(str, Boolean.TrueString, StringComparison.OrdinalIgnoreCase)) return true;
                if (String.Equals(str, Boolean.FalseString, StringComparison.OrdinalIgnoreCase)) return false;

                // 特殊处理用数字0和1表示布尔型
                str = ToDBC(str);
                if (Int32.TryParse(str, out var n)) return n > 0;

                return defaultValue;
            }

            try
            {
                return Convert.ToBoolean(value);
            }
            catch { return defaultValue; }
        }

        /// <summary>转为时间日期，转换失败时返回最小时间。支持字符串、整数（Unix秒）</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public virtual DateTime ToDateTime(Object value, DateTime defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的

            if (value is String str)
            {
                //str = ToDBC(str).Trim();
                str = str.Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                // 处理UTC
                var utc = false;
                if (str.EndsWithIgnoreCase(" UTC"))
                {
                    utc = true;
                    str = str.Substring(0, str.Length - 4);
                }

                var dt = DateTime.MinValue;
                if (!DateTime.TryParse(str, out dt) &&
                    !str.Contains("-") && DateTime.TryParseExact(str, "yyyy-M-d", null, DateTimeStyles.None, out dt) &&
                    !str.Contains("/") && DateTime.TryParseExact(str, "yyyy/M/d", null, DateTimeStyles.None, out dt) &&
                    !DateTime.TryParse(str, out dt))
                {
                    dt = defaultValue;
                }

                // 处理UTC
                if (utc) dt = new DateTime(dt.Ticks, DateTimeKind.Utc);

                return dt;
            }
            // 特殊处理整数，Unix秒，绝对时间差，不考虑UTC时间和本地时间。
            if (value is Int32 k) return k == 0 ? DateTime.MinValue : _dt1970.AddSeconds(k);
            if (value is Int64 m)
            {
                if (m > 100 * 365 * 24 * 3600L)
                    return _dt1970.AddMilliseconds(m);
                else
                    return _dt1970.AddSeconds(m);
            }

            try
            {
                return Convert.ToDateTime(value);
            }
            catch { return defaultValue; }
        }

        /// <summary>转为时间日期，转换失败时返回最小时间。支持字符串、整数（Unix秒）</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public virtual DateTimeOffset ToDateTimeOffset(Object value, DateTimeOffset defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            // 特殊处理字符串，也是最常见的

            if (value is String str)
            {
                str = str.Trim();
                if (str.IsNullOrEmpty()) return defaultValue;

                if (DateTimeOffset.TryParse(str, out var dt)) return dt;
                if (str.Contains("-") && DateTimeOffset.TryParseExact(str, "yyyy-M-d", null, DateTimeStyles.None, out dt)) return dt;
                if (str.Contains("/") && DateTimeOffset.TryParseExact(str, "yyyy/M/d", null, DateTimeStyles.None, out dt)) return dt;

                return defaultValue;
            }
            // 特殊处理整数，Unix秒，绝对时间差，不考虑UTC时间和本地时间。
            if (value is Int32 k) return k == 0 ? DateTimeOffset.MinValue : _dto1970.AddSeconds(k);
            if (value is Int64 m)
            {
                if (m > 100 * 365 * 24 * 3600L)
                    return _dto1970.AddMilliseconds(m);
                else
                    return _dto1970.AddSeconds(m);
            }

            try
            {
                return Convert.ToDateTime(value);
            }
            catch { return defaultValue; }
        }

        /// <summary>全角为半角</summary>
        /// <remarks>全角半角的关系是相差0xFEE0</remarks>
        /// <param name="str"></param>
        /// <returns></returns>
        String ToDBC(String str)
        {
            var ch = str.ToCharArray();
            for (var i = 0; i < ch.Length; i++)
            {
                // 全角空格
                if (ch[i] == 0x3000)
                    ch[i] = (Char)0x20;
                else if (ch[i] > 0xFF00 && ch[i] < 0xFF5F)
                    ch[i] = (Char)(ch[i] - 0xFEE0);
            }
            return new String(ch);
        }

        /// <summary>时间日期转为yyyy-MM-dd HH:mm:ss完整字符串</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="emptyValue">字符串空值时显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns></returns>
        public virtual String ToFullString(DateTime value, String emptyValue = null)
        {
            if (emptyValue != null && value <= DateTime.MinValue) return emptyValue;

            //return value.ToString("yyyy-MM-dd HH:mm:ss");

            //var dt = value;
            //var sb = new StringBuilder();
            //sb.Append(dt.Year.ToString().PadLeft(4, '0'));
            //sb.Append("-");
            //sb.Append(dt.Month.ToString().PadLeft(2, '0'));
            //sb.Append("-");
            //sb.Append(dt.Day.ToString().PadLeft(2, '0'));
            //sb.Append(" ");

            //sb.Append(dt.Hour.ToString().PadLeft(2, '0'));
            //sb.Append(":");
            //sb.Append(dt.Minute.ToString().PadLeft(2, '0'));
            //sb.Append(":");
            //sb.Append(dt.Second.ToString().PadLeft(2, '0'));

            //return sb.ToString();

            var cs = "yyyy-MM-dd HH:mm:ss".ToCharArray();

            var k = 0;
            var y = value.Year;
            cs[k++] = (Char)('0' + (y / 1000));
            y %= 1000;
            cs[k++] = (Char)('0' + (y / 100));
            y %= 100;
            cs[k++] = (Char)('0' + (y / 10));
            y %= 10;
            cs[k++] = (Char)('0' + y);
            k++;

            var m = value.Month;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Day;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Hour;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Minute;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Second;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            var str = new String(cs);

            if (value.Kind == DateTimeKind.Utc) str += " UTC";

            return str;
        }

        /// <summary>时间日期转为yyyy-MM-dd HH:mm:ss完整字符串</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="emptyValue">字符串空值时显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns></returns>
        public virtual String ToFullString(DateTimeOffset value, String emptyValue = null)
        {
            if (emptyValue != null && value <= DateTimeOffset.MinValue) return emptyValue;

            var cs = "yyyy-MM-dd HH:mm:ss +08:00".ToCharArray();

            var k = 0;
            var y = value.Year;
            cs[k++] = (Char)('0' + (y / 1000));
            y %= 1000;
            cs[k++] = (Char)('0' + (y / 100));
            y %= 100;
            cs[k++] = (Char)('0' + (y / 10));
            y %= 10;
            cs[k++] = (Char)('0' + y);
            k++;

            var m = value.Month;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Day;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Hour;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Minute;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            m = value.Second;
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;

            // 时区
            var offset = value.Offset;
            cs[k++] = offset.TotalSeconds >= 0 ? '+' : '-';
            m = Math.Abs(offset.Hours);
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;
            m = Math.Abs(offset.Minutes);
            cs[k++] = (Char)('0' + (m / 10));
            cs[k++] = (Char)('0' + (m % 10));

            return new String(cs);
        }

        /// <summary>时间日期转为指定格式字符串</summary>
        /// <param name="value">待转换对象</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="emptyValue">字符串空值时显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns></returns>
        public virtual String ToString(DateTime value, String format, String emptyValue)
        {
            if (emptyValue != null && value <= DateTime.MinValue) return emptyValue;

            //return value.ToString(format ?? "yyyy-MM-dd HH:mm:ss");

            if (format == null || format == "yyyy-MM-dd HH:mm:ss") return ToFullString(value, emptyValue);

            return value.ToString(format);
        }

        /// <summary>获取内部真实异常</summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual Exception GetTrue(Exception ex)
        {
            if (ex == null) return null;

            if (ex is AggregateException)
                return GetTrue((ex as AggregateException).Flatten().InnerException);

            if (ex is TargetInvocationException)
                return GetTrue((ex as TargetInvocationException).InnerException);

            if (ex is TypeInitializationException)
                return GetTrue((ex as TypeInitializationException).InnerException);

            return ex.GetBaseException() ?? ex;
        }

        /// <summary>获取异常消息</summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public virtual String GetMessage(Exception ex)
        {
            var msg = ex + "";
            if (msg.IsNullOrEmpty()) return null;

            var ss = msg.Split(Environment.NewLine);
            var ns = ss.Where(e =>
            !e.StartsWith("---") &&
            !e.Contains("System.Runtime.ExceptionServices") &&
            !e.Contains("System.Runtime.CompilerServices"));

            msg = ns.Join(Environment.NewLine);

            return msg;
        }

        /// <summary>字节单位字符串</summary>
        /// <param name="value">数值</param>
        /// <param name="format">格式化字符串</param>
        /// <returns></returns>
        public virtual String ToGMK(Int64 value, String format = null)
        {
            if (value < 1024) return "{0:n0}".F(value);

            if (format.IsNullOrEmpty()) format = "{0:n0}";

            var val = value / 1024d;
            if (val < 1024) return format.F(val) + "K";

            val /= 1024;
            if (val < 1024) return format.F(val) + "M";

            val /= 1024;
            if (val < 1024) return format.F(val) + "G";

            val /= 1024;
            if (val < 1024) return format.F(val) + "T";

            val /= 1024;
            if (val < 1024) return format.F(val) + "P";

            val /= 1024;
            return format.F(val) + "E";
        }
    }
}
