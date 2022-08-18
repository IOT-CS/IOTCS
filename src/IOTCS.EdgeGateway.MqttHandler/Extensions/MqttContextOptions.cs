namespace IOTCS.EdgeGateway.MqttHandler.Extensions
{
    public class MqttContextOptions
    {
        /// <summary>
        /// 是否使用用户名和密码
        /// </summary>
        public int IsUsingUser { get; set; }

        /// <summary>
        /// Mqtt Ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 客户端ID号
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }
    }
}
