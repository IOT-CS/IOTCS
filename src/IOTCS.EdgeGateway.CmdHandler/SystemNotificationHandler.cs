using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Logging;
using MediatR;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Diagnostics.Notification;
using IOTCS.EdgeGateway.DotNetty;

namespace IOTCS.EdgeGateway.CmdHandler
{
    public class SystemNotificationHandler : INotificationHandler<SystemNotification>
    {
        private readonly ILogger _logger;
        private readonly IWebSocketServer _webSocket;

        public SystemNotificationHandler()
        {
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("WebSocket");
            _webSocket = IocManager.Instance.GetService<IWebSocketServer>();
        }

        public Task Handle(SystemNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var strString = HandleMessage(notification);
                Send(strString).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _logger.Error($"OPCUA 系统消息解析异常，错误消息 => {e.Message},错误位置 => {e.StackTrace},");
            }           

            return Task.CompletedTask;
        }

        private async Task Send(string msg)
        {
            try
            {
                var connections = _webSocket?.GetAllConnections();
                if (connections != null && connections.Count > 0)
                {
                    foreach (var conn in connections)
                    {
                        await conn.Send(msg);
                        
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"OPCUA 系统消息发送WebSocket信息到UI异常，异常信息 => {e.Message},位置 => {e.StackTrace}");
            }
        }

        private string HandleMessage(SystemNotification message)
        {
            var result = string.Empty;
            var socketObject = new WebSocketProtocol<string>();

            if (message != null)
            {
                socketObject.RequestType = message.MsgType;
                socketObject.Data = message.Message;
                result = JsonConvert.SerializeObject(socketObject);
            }

            return result;
        }
    }
}
