using IOTCS.EdgeGateway.Diagnostics.Notification;
using IOTCS.EdgeGateway.Logging;
using MediatR;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Diagnostics
{
    public class SystemDiagnostics : ISystemDiagnostics
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public SystemDiagnostics(ILogger logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// 发送订阅数据到界面展示
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(string message)
        {
            var command = new SystemNotification
            {
                Message = message,
                MsgType = "subLog"
            };
            await _mediator.Publish<SystemNotification>(command);
        }

        /// <summary>
        /// 发送诊断信息到界面展示
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishDiagnosticsInfoAsync(string message)
        {
            var command = new SystemNotification 
            {
                Message = message,
                MsgType = "diagnostics"
            };
            await _mediator.Publish<SystemNotification>(command);            
        }
    }
}
