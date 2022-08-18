using System;

namespace IOTCS.EdgeGateway.CmdHandler
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
        /// pong            心跳
        /// dashboard     仪表盘信息
        /// nodeVaule       点位信息
        /// diagnosticLog   诊断日志
        /// subLog          系统日志
        /// 
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        public T Data { get; set; }
    }
}
