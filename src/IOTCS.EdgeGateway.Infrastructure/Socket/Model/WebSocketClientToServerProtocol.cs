namespace IOTCS.EdgeGateway.Infrastructure.Socket
{
    public class WebSocketClientToServerProtocol<T>
    {
        public string RequestID { get; set; }

        /// <summary>
        /// 自定义操作类型，以下操作已经封装
        ///1.connected 表示服务端主动推送
        ///2.ping 客户端发送心跳包到服务端，服务端返回RequestType=pong
        ///3.close 服务器端异常发送RequestType = close 到客户端
        /// </summary>
        public string RequestType { get; set; }

        public T Data { get; set; }
    }
}
