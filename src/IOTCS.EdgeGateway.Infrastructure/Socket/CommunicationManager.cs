using IOTCS.EdgeGateway.DotNetty;
using IOTCS.EdgeGateway.Logging;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Infrastructure.Socket
{
    public class CommunicationManager
    {
        private ILogger _logger = null;
        private IMediator _mediator;

        public CommunicationManager(ILogger logger, IMediator mediator)
        {           
            _logger = logger;
            _mediator = mediator;
        }
     
        public async Task<IWebSocketServer> WebSocketServerBuilder(SocketOptions options)
        {
            _logger.Info($"启动WebSocket参数 => {JsonConvert.SerializeObject(options)}");
            var theServer = await SocketBuilderFactory.GetWebSocketServerBuilder(options)
                .OnConnectionClose((server, connection) =>
                {
                    _logger.Info($"connect closed,{connection.ConnectionId}");
                    connection.Close();
                })
                .OnException(ex =>
                {
                    _logger.Error($"WebSocket 服务端异常:{ex.Message}||{ex.StackTrace}");
                })
                .OnNewConnection((server, connection) =>
                {
                    _logger.Info($"WebSocket 建立新连接:{connection.ConnectionName}");
                })
                .OnRecieve(async (server, connection, msg) =>
                {
                    //处理客户端发送过来的数据
                    await ProcessAsync(server, connection, msg);
                })
                .OnServerStarted(server =>
                {
                    _logger.Info($"WebSocket 服务启动,监听端口：{server.Port}");
                }).BuildAsync();

            return await Task.FromResult(theServer);
        }

        private async Task ProcessAsync(IWebSocketServer server,IWebSocketConnection connection, string msg)
        {
            try
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    var protocol = JsonConvert
                        .DeserializeObject<WebSocketClientToServerProtocol<NullClientToServerProtocol>>(msg);

                    switch (protocol.RequestType)
                    {
                        case "ping":
                            var sendProtocol = new WebSocketProtocol<NullClientToServerProtocol>();
                            sendProtocol.RequestType = "pong";
                            var sendMsg = JsonConvert.SerializeObject(sendProtocol);
                            await SendAsync(server, sendMsg);
                            break;
                        case "close":
                            _logger.Info($"connect closed,protocol.RequestType => {protocol.RequestType}");
                            connection.Close();
                            break;
                        default://广播消息给所有设备
                            //var command = new WebSocketNotificationCommand { Message = msg };
                            //await _mediator.Publish<WebSocketNotificationCommand>(command);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"WebSocket处理客户端发送过来的数据时异常:{ex.Message}||{ex.StackTrace}");
            }
            
            await Task.CompletedTask;
        }

        private async Task SendAsync(IWebSocketServer server, string msg)
        {
            var connections = server?.GetAllConnections();
            if (connections != null && connections.Count > 0)
            {
                foreach (var conn in connections)
                {                    
                    await conn.Send(msg);
                }
            }
        }
    }
}
