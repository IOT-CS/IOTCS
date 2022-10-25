using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Diagnostics.Notification;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.WsHandler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.CmdHandler
{
    public class SystemNotificationHandler : INotificationHandler<SystemNotification>
    {
        private readonly ILogger _logger;
        private WsMessageHandler _webSocket;

        public SystemNotificationHandler(WsMessageHandler messageHandler)
        {
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("WebSocket");
            _webSocket = messageHandler;
        }

        public async Task Handle(SystemNotification notification, CancellationToken cancellationToken)
        {
            try
            {                
                await Send(notification);
            }
            catch (Exception e)
            {
                _logger.Error($"系统消息通知异常，错误消息 => {e.Message},错误位置 => {e.StackTrace},");
            }          
        }

        private async Task Send(SystemNotification message)
        {
            try
            {
                await _webSocket.SendMessageToAllAsync(new WebSocketManager.Common.Message
                {
                    MessageType = WebSocketManager.Common.MessageType.Text,
                    RequestType = message.MsgType,
                    Data = message.Message
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.Error($"系统消息发送WebSocket信息到UI异常，异常信息 => {e.Message},位置 => {e.StackTrace}");
            }            
        }
    }
}
