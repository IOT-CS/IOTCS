using System;

namespace IOTCS.EdgeGateway.Infrastructure.Socket
{
    public class WebSocketProtocol<T>
    {
        /// <summary>
        /// 消息流水号
        /// </summary>
        public string RequestId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 请求类型
        /// 
        /// pong
        /// sysInfo
        /// connection
        /// nodeVaule
        /// diagnosticLog
        /// subLog
        /// 
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        public T Data { get; set; }
    }
}
