using System;
using System.IO;
using Volo.Abp.IO;

namespace System
{
    public static class IntExtensions
    {
        public static DefaultConvert Convert { get; set; } = new DefaultConvert();

        /// <summary>从字节数据指定位置读取一个无符号32位整数</summary>
        /// <param name="data"></param>
        /// <param name="offset">偏移</param>
        /// <param name="isLittleEndian">是否小端字节序</param>
        /// <returns></returns>
        public static UInt32 ToUInt32(this Byte[] data, Int32 offset = 0, Boolean isLittleEndian = true)
        {
            if (isLittleEndian) return BitConverter.ToUInt32(data, offset);

            // BitConverter得到小端，如果不是小端字节顺序，则倒序
            if (offset > 0) data = data.ReadBytes(offset, 4);
            if (isLittleEndian)
                return (UInt32)(data[0] | data[1] << 8 | data[2] << 0x10 | data[3] << 0x18);
            else
                return (UInt32)(data[0] << 0x18 | data[1] << 0x10 | data[2] << 8 | data[3]);
        }

        /// <summary>从字节数据指定位置读取一个无符号64位整数</summary>
        /// <param name="data"></param>
        /// <param name="offset">偏移</param>
        /// <param name="isLittleEndian">是否小端字节序</param>
        /// <returns></returns>
        public static UInt64 ToUInt64(this Byte[] data, Int32 offset = 0, Boolean isLittleEndian = true)
        {
            if (isLittleEndian) return BitConverter.ToUInt64(data, offset);

            if (offset > 0) data = data.ReadBytes(offset, 8);
            if (isLittleEndian)
            {
                var num1 = data[0] | data[1] << 8 | data[2] << 0x10 | data[3] << 0x18;
                var num2 = data[4] | data[5] << 8 | data[6] << 0x10 | data[7] << 0x18;
                return (UInt32)num1 | (UInt64)num2 << 0x20;
            }
            else
            {
                var num3 = data[0] << 0x18 | data[1] << 0x10 | data[2] << 8 | data[3];
                var num4 = data[4] << 0x18 | data[5] << 0x10 | data[6] << 8 | data[7];
                return (UInt32)num4 | (UInt64)num3 << 0x20;
            }
        }



        /// <summary>转为整数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）、时间（Unix秒）</summary>
        /// <remarks>Int16/UInt32/Int64等，可以先转为最常用的Int32后再二次处理</remarks>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns></returns>
        public static Int32 ToInt(this Object value, Int32 defaultValue = 0) => Convert.ToInt(value, defaultValue);
    }
}
