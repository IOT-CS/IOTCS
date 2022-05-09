using IOTCS.EdgeGateway.DotNetty;
using IOTCS.EdgeGateway.Logging;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Infrastructure.Socket
{
    public class AbpWebSocketModule : AbpModule
    {
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var serviceProvider = context.Services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            SocketOptions options = new SocketOptions
            {
                IpAddress = configuration["WebSocketServer:IpAddress"],
                Port = Convert.ToInt32(configuration["WebSocketServer:Port"]),
                Path = configuration["WebSocketServer:Path"]
            };
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("WebSocket");
            var mediator = serviceProvider.GetService<IMediator>();
            CommunicationManager socket = new CommunicationManager(logger, mediator);
            var server = socket.WebSocketServerBuilder(options).GetAwaiter().GetResult();
            context.Services.Add(ServiceDescriptor.Singleton<IWebSocketServer>(server));
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {

        }
    }
}
