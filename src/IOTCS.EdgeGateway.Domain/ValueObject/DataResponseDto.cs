using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class DataResponseDto<T>
    {
        /// <summary>
        /// true:成功
        /// false: 失败
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// 系统出错信息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 系统错误编号
        /// 0: 正确值
        /// </summary>
        public int ErrorCode { get; set; } = 0;

        /// <summary>
        /// 详细数据信息
        /// </summary>
        public T Data { get; set; }
    }
}
