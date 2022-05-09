namespace IOTCS.EdgeGateway.Infrastructure.Socket
{
    public class WebSocketServerOptions
    {
        public string IpAddress { get; set; } = "*";

        public int Port { get; set; } = 6003;
    }
}
