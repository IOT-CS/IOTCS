namespace IOTCS.EdgeGateway.WebSocketManager.Common
{
    public enum MessageType
    {
        Text,
        MethodInvocation,
        ConnectionEvent,
        MethodReturnValue
    }

    public class Message
    {
        public MessageType MessageType { get; set; }

        public string RequestType { get; set; } = "No";

        public dynamic Data { get; set; }
    }
}