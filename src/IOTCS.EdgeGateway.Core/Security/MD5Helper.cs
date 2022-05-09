using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace IOTCS.EdgeGateway.Core.Security
{
    public class MD5Helper
    {
        /// <summary>
        /// 生成32位大写MD5加密字符串
        /// </summary>
        /// <param name="inputValue">输入值</param>
        /// <returns></returns>
        public static string GenerateMd5String(string inputValue)
        {
            var result = string.Empty;

            //32位大写
            using (var md5 = MD5.Create())
            {
                var hashResult = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                var strResult = BitConverter.ToString(hashResult);
                result = strResult.Replace("-", "");
            }

            return result;
        }

        public static string GenerateMd5Token(string name)
        {
            var result = string.Empty;
            var saltString = "@#$%unilever*&";

            //32位大写
            using (var md5 = MD5.Create())
            {
                saltString = saltString + name;
                var hashResult = md5.ComputeHash(Encoding.UTF8.GetBytes(saltString));
                var strResult = BitConverter.ToString(hashResult);
                result = strResult.Replace("-", "");
            }

            return result;
        }
    }
}
