using IOTCS.EdgeGateway.WebSocketManager;
using IOTCS.EdgeGateway.WebSocketManager.Common;

namespace IOTCS.EdgeGateway.WsHandler
{
    public class WsMessageHandler : WebSocketHandler
    {
        public WsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) 
            : base(webSocketConnectionManager, new StringMethodInvocationStrategy())
        {

        }
    }
}
